using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PlayOfferService.Application;
using PlayOfferService.Domain;
using PlayOfferService.Domain.Repositories;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var readConnectionString = "Host=pos_postgres_read;Database=pos_read_db;Username=pos_user;Password=pos_password";
var writeConnectionString = "Host=pos_postgres_write;Database=pos_write_db;Username=pos_user;Password=pos_password;";
builder.Services.AddDbContext<DbReadContext>(options =>
    {
        options.UseNpgsql(readConnectionString);
        options.UseCamelCaseNamingConvention();
    }
);

builder.Services.AddDbContext<DbWriteContext>(options =>
    {
        options.UseNpgsql(writeConnectionString);
        options.UseCamelCaseNamingConvention();
    }
);

// Add services to the container.
builder.Services.AddScoped<ClubRepository>();
builder.Services.AddScoped<MemberRepository>();
builder.Services.AddScoped<PlayOfferRepository>();
builder.Services.AddScoped<ReservationRepository>();
builder.Services.AddScoped<CourtRepository>();
builder.Services.AddScoped<ReadEventRepository>();
builder.Services.AddScoped<WriteEventRepository>();
builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

if (builder.Environment.EnvironmentName != "Test")
{
    builder.Services.AddHostedService<RedisPlayOfferStreamService>();
    builder.Services.AddHostedService<RedisClubStreamService>();
    builder.Services.AddHostedService<RedisMemberStreamService>();
    builder.Services.AddHostedService<RedisReservationStreamService>();
    builder.Services.AddHostedService<RedisCourtStreamService>();
}

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "PlayOfferService API",
        Description = "An ASP.NET Core Web API for managing PlayOffers",
    });

    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "PlayofferService API v1");
});

// Create the database if it doesn't exist
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var readDbContext = services.GetRequiredService<DbReadContext>();
    var writeDbContext = services.GetRequiredService<DbWriteContext>();
    
    readDbContext.Database.EnsureCreated();
    writeDbContext.Database.EnsureCreated();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Needed for integration tests
public abstract partial class Startup
{
}