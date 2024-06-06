using System.Text.Json.Nodes;
using MediatR;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Court;
using StackExchange.Redis;

namespace PlayOfferService.Application;

public class RedisCourtStreamService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly CancellationToken _cancellationToken;
    private readonly IDatabase _db;
    private const string StreamName = "court_service.events.baseevents";
    private const string GroupName = "pos.courts.events.group";
    
    
    public RedisCourtStreamService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        var tokenSource = new CancellationTokenSource();
        _cancellationToken = tokenSource.Token;
        var muxer = ConnectionMultiplexer.Connect("pos_redis");
        _db = muxer.GetDatabase();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        if (!(await _db.KeyExistsAsync(StreamName)) ||
            (await _db.StreamGroupInfoAsync(StreamName)).All(x=>x.Name!=GroupName))
        {
            await _db.StreamCreateConsumerGroupAsync(StreamName, GroupName, "0-0");
        }
        

        var id = string.Empty;
        while (!_cancellationToken.IsCancellationRequested)
        {
            if (!string.IsNullOrEmpty(id))
            {
                await _db.StreamAcknowledgeAsync(StreamName, GroupName, id);
                id = string.Empty;
            }
            var result = await _db.StreamReadGroupAsync(StreamName, GroupName, "pos-club", ">", 1);
            if (result.Any())
            {
                var streamEntry = result.First();
                id = streamEntry.Id;
                var parsedEvent = FilterandParseEvent(streamEntry);
                if (parsedEvent == null)
                    continue;
                await mediator.Send(parsedEvent, _cancellationToken);
            }
            await Task.Delay(1000);
        }

    }
    
    private TechnicalCourtEvent? FilterandParseEvent(StreamEntry value)
    {
        var dict = value.Values.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
        var jsonContent = JsonNode.Parse(dict.Values.First());
        var eventInfo = JsonNode.Parse(jsonContent["payload"]["after"].GetValue<string>());
        
        var eventType = eventInfo["eventType"].GetValue<string>();
        var entityType = eventInfo["entityType"].GetValue<string>();
        var acceptedEventTypes = new List<string>
        {
            "CourtCreatedEvent",
            "CourtUpdatedEvent"
        };
        
        if (!acceptedEventTypes.Contains(eventType) || entityType != "Court")
        {
            return null;
        }
        
        return EventParser.ParseEvent<TechnicalCourtEvent>(eventInfo);
    }
}