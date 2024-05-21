using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PlayOfferService.Application;
using PlayOfferService.Domain.Repositories;
using PlayOfferService.Repositories;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var connectionString = "Host=pos_postgres;Database=pos_db;Username=pos_user;Password=pos_password";

var readConnectionString = "Host=pos_postgres;Database=pos_read_db;Username=pos_user;Password=pos_password";
var writeConnectionString = "Host=pos_postgres;Database=pos_write_db;Username=pos_user;Password=pos_password";

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(connectionString)
);

builder.Services.AddDbContext<ReadDbContext>(options =>
    options.UseNpgsql(readConnectionString)
);

builder.Services.AddDbContext<WriteDbContext>(options =>
    options.UseNpgsql(writeConnectionString)
);

builder.Services.AddScoped(serviceProvider => new CQRSContexts(
    serviceProvider.GetRequiredService<ReadDbContext>(),
    serviceProvider.GetRequiredService<WriteDbContext>())
);

// Add services to the container.
builder.Services.AddScoped<ClubRepository>();
builder.Services.AddScoped<MemberRepository>();
builder.Services.AddScoped<PlayOfferRepository>();
builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddHostedService<RedisPlayOfferStreamService>();
builder.Services.AddHostedService<RedisClubStreamService>();
builder.Services.AddHostedService<RedisMemberStreamService>();

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

    options.IncludeXmlComments(Path.Combine(System.AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "PlayofferService API v1");
});

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var dbContext = services.GetRequiredService<DatabaseContext>();
var readDbContext = services.GetRequiredService<ReadDbContext>();
var writeDbContext = services.GetRequiredService<WriteDbContext>();

// Create the database if it doesn't exist
dbContext.Database.EnsureCreated();
readDbContext.Database.EnsureCreated();
writeDbContext.Database.EnsureCreated();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
