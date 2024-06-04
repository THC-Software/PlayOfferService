using System.Text.Json;
using System.Text.Json.Nodes;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Repositories;
using StackExchange.Redis;

namespace PlayOfferService.Application;

public class RedisReservationStreamService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly CancellationToken _cancellationToken;
    private readonly IDatabase _db;
    private const string StreamName = "court_service.test.baseevents";
    private const string GroupName = "pos.reservation.events.group";
    
    
    public RedisReservationStreamService(IServiceScopeFactory serviceScopeFactory)
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
        PlayOfferRepository playOfferRepository = scope.ServiceProvider.GetRequiredService<PlayOfferRepository>();
        
        if (!(await _db.KeyExistsAsync(StreamName)) ||
            (await _db.StreamGroupInfoAsync(StreamName)).All(x=>x.Name!=GroupName))
        {
            await _db.StreamCreateConsumerGroupAsync(StreamName, GroupName, "0-0", true);
        }
        
        var id = string.Empty;
        while (!_cancellationToken.IsCancellationRequested)
        {
            if (!string.IsNullOrEmpty(id))
            {
                await _db.StreamAcknowledgeAsync(StreamName, GroupName, id);
                id = string.Empty;
            }
            var result = await _db.StreamReadGroupAsync(StreamName, GroupName, "pos-member", ">", 1);
            if (result.Any())
            {
                var streamEntry = result.First();
                id = streamEntry.Id;
                var parsedEvent = ParseEvent(streamEntry);
                if (parsedEvent == null)
                    continue;
                
                if(parsedEvent.CorrelationId!=null)
                    await playOfferRepository.UpdateEntityAsync(parsedEvent);
                
                // TODO: Add reservation projection
            }
            await Task.Delay(1000);
        }
    }
    
    private BaseEvent? ParseEvent(StreamEntry value)
    {
        var dict = value.Values.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
        var jsonContent = JsonNode.Parse(dict.Values.First());
        var eventInfo = JsonNode.Parse(jsonContent["payload"]["after"].GetValue<string>());
        
        var eventType = eventInfo["eventType"].GetValue<string>();
        
        if (eventType != "ReservationCreatedEvent")
        {
            return null;
        }

        return EventParser.ParseEvent(eventInfo);
    }
}