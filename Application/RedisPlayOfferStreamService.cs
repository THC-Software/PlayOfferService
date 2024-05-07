using PlayOfferService.Repositories;
using StackExchange.Redis;

namespace PlayOfferService.Application;

public class RedisPlayOfferStreamService : BackgroundService
{
    private readonly PlayOfferRepository _playOfferRepository;
    private Task _readTask;
    private readonly CancellationToken _cancellationToken;
    private readonly ConnectionMultiplexer _muxer;
    private readonly IDatabase _db;
    private const string StreamName = "pos.public.events";
    private const string GroupName = "pos.domain.events.group";
    
    
    public RedisPlayOfferStreamService()
    {
        _playOfferRepository = null;
        var tokenSource = new CancellationTokenSource();
        _cancellationToken = tokenSource.Token;
        _muxer = ConnectionMultiplexer.Connect("pos_redis");
        _db = _muxer.GetDatabase();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
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
                var result = await _db.StreamReadGroupAsync(StreamName, GroupName, "avg-1", ">", 1);
                if (result.Any())
                {
                    var event = ParseEvent(result.First());
                }
                await Task.Delay(1000);
            }
        });
    }
    
    private void ParseEvent(StreamEntry value)
    {
        var dict = value.Values.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
        Console.WriteLine(dict.Values.ToString());
    }
}