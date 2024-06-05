using System.Text.Json.Nodes;
using MediatR;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Repositories;
using StackExchange.Redis;

namespace PlayOfferService.Application;

public class RedisClubStreamService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly CancellationToken _cancellationToken;
    private readonly IDatabase _db;
    private const string StreamName = "club_service_events.public.DomainEvent";
    private const string GroupName = "pos.club.events.group";
    
    
    public RedisClubStreamService(IServiceScopeFactory serviceScopeFactory)
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
                await mediator.Publish(parsedEvent, _cancellationToken);
            }
            await Task.Delay(1000);
        }

    }
    
    private TechnicalClubEvent? FilterandParseEvent(StreamEntry value)
    {
        var dict = value.Values.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
        var jsonContent = JsonNode.Parse(dict.Values.First());
        var eventInfo = jsonContent["payload"]["after"];
        
        var eventType = eventInfo["eventType"].GetValue<string>();
        var entityType = eventInfo["entityType"].GetValue<string>();
        
        if ((eventType != "TENNIS_CLUB_REGISTERED"
            && eventType != "TENNIS_CLUB_LOCKED"
            && eventType != "TENNIS_CLUB_UNLOCKED"
            && eventType != "TENNIS_CLUB_DELETED") || entityType != "TENNIS_CLUB")
        {
            return null;
        }
        
        return EventParser.ParseEvent<TechnicalClubEvent>(eventInfo);
    }
}