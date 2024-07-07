using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PlayOfferService.Application;
using PlayOfferService.Domain;
using PlayOfferService.Domain.Repositories;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using PlayOfferService;

var builder = WebApplication.CreateBuilder(args);

var readConnectionString = "Host=pos-postgres-read;Database=pos_read_db;Username=pos_user;Password=pos_password";
var writeConnectionString = "Host=pos-postgres-write;Database=pos_write_db;Username=pos_user;Password=pos_password;";
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

builder.Services
    .AddAuthentication("BasicScheme")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicScheme", null);
builder.Services.AddAuthorization();

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
    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "PlayofferService API v1");
});


using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var readDbContext = services.GetRequiredService<DbReadContext>();
var writeDbContext = services.GetRequiredService<DbWriteContext>();

readDbContext.Database.EnsureCreated();
writeDbContext.Database.EnsureCreated();


app.UseMiddleware<JwtClaimsMiddleware>(); // Use custom middleware to extract JWT claims
app.UseAuthorization();

app.MapControllers();

app.Run();

// Needed for integration tests
public abstract partial class Startup
{
}