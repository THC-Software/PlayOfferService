using Microsoft.Extensions.DependencyInjection;
using PlayOfferService.Domain;
using PlayOfferService.Domain.Repositories;
using Testcontainers.PostgreSql;

namespace PlayOfferService.Tests;

public class TestSetup
{
    private DbReadContext _readContext;
    private DbWriteContext _writeContext;
    private WebAppFactory _factory;
    private PostgreSqlContainer _readDbContainer;
    private PostgreSqlContainer _writeDbContainer;
    protected ClubRepository TestClubRepository;
    protected MemberRepository TestMemberRepository;
    protected HttpClient HttpClient;
    
    [SetUp]
    public async Task Setup()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
        
        _readDbContainer = new PostgreSqlBuilder()
            .WithImage("debezium/postgres:16-alpine")
            .WithUsername("pos_user")
            .WithPassword("pos_password")
            .WithDatabase("pos_read_test")
            .WithPortBinding(5432, true)
            .Build();
        _writeDbContainer = new PostgreSqlBuilder()
            .WithImage("debezium/postgres:16-alpine")
            .WithUsername("pos_user")
            .WithPassword("pos_password")
            .WithDatabase("pos_write_test")
            .WithPortBinding(5432, true)
            .Build();
        
        await _readDbContainer.StartAsync();
        await _writeDbContainer.StartAsync();
        
        _factory = new WebAppFactory(_readDbContainer.GetConnectionString(), _writeDbContainer.GetConnectionString());
        HttpClient = _factory.CreateClient();
        
        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>() ??
                           throw new Exception("Scope factory not found");
        var scope = scopeFactory.CreateScope() ??
                    throw new Exception("Could not create Scope");

        _readContext = scope.ServiceProvider.GetService<DbReadContext>() ??
                       throw new Exception("Could not get DbReadContext");
        _writeContext = scope.ServiceProvider.GetService<DbWriteContext>() ??
                       throw new Exception("Could not get DbWriteContext");
        
        TestClubRepository = scope.ServiceProvider.GetService<ClubRepository>() ??
                          throw new Exception("Could not get ClubRepository");

        TestMemberRepository = scope.ServiceProvider.GetService<MemberRepository>() ??
                               throw new Exception("Could not get MemberRepository");
                               
        _readContext.Database.EnsureCreated();
        _writeContext.Database.EnsureCreated();
    }
    
    [TearDown]
    public async Task TearDown()
    {
        HttpClient.Dispose();
        await _factory.DisposeAsync();
        await _readContext.DisposeAsync();
        await _writeContext.DisposeAsync();
        await _readDbContainer.DisposeAsync();
        await _writeDbContainer.DisposeAsync();
    }
}