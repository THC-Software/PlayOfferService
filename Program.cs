using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PlayOfferService.Repositories;

var builder = WebApplication.CreateBuilder(args);
var connectionString = "server=pos_mysql;port=3306;user=pos_user;password=pos_pw;database=defaultdb;";
var serverVersion = ServerVersion.AutoDetect(connectionString);

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseMySql(connectionString, serverVersion).UseCamelCaseNamingConvention()
);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
dbContext.Database.EnsureCreated();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
