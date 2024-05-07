using System.Text.Json;
using System.Text.Json.Nodes;
using PlayOfferService.Domain.Events;
using PlayOfferService.Repositories;
using StackExchange.Redis;

namespace PlayOfferService.Application;

public class RedisPlayOfferStreamService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private Task? _readTask;
    private readonly CancellationToken _cancellationToken;
    private readonly IDatabase _db;
    private const string StreamName = "pos.public.events";
    private const string GroupName = "pos.domain.events.group";
    
    
    public RedisPlayOfferStreamService(IServiceScopeFactory serviceScopeFactory)
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
        
        _readTask = Task.Run(async () =>
        {
            string id = string.Empty;
            while (!_cancellationToken.IsCancellationRequested)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    await _db.StreamAcknowledgeAsync(StreamName, GroupName, id);
                    id = string.Empty;
                }
                var result = await _db.StreamReadGroupAsync(StreamName, GroupName, "pos-1", ">", 1);
                if (result.Any())
                {
                    var parsedEvent = ParseEvent(result.First());
                    await playOfferRepository.UpdateEntityAsync(parsedEvent);
                }
                await Task.Delay(1000);
            }
        }, stoppingToken);
    }
    
    private BaseEvent ParseEvent(StreamEntry value)
    {
        var dict = value.Values.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
        var jsonContent = JsonNode.Parse(dict.Values.First());
        var eventInfo = jsonContent["payload"]["after"];
        
        var baseEvent = new BaseEvent
        {
            EventId = Guid.Parse(eventInfo["EventId"].GetValue<string>()),
            EventType = (EventType)Enum.Parse(typeof(EventType), eventInfo["EventType"].GetValue<string>()),
            Timestamp = DateTime.Parse(eventInfo["Timestamp"].GetValue<string>()),
            EntityId = Guid.Parse(eventInfo["EntityId"].GetValue<string>()),
            EntityType = (EntityType)Enum.Parse(typeof(EntityType), eventInfo["EntityType"].GetValue<string>()),
            EventData = JsonSerializer.Deserialize<IDomainEvent>(eventInfo["EventData"].GetValue<string>(), JsonSerializerOptions.Default),
        };
        
        return baseEvent;
    }
}