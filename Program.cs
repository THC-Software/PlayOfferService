using Microsoft.EntityFrameworkCore;
using PlayOfferService.Repositories;

var builder = WebApplication.CreateBuilder(args);
var connectionString = "server=playofferservice-db-do-user-14755325-0.c.db.ondigitalocean.com;port=25060;user=doadmin;password=AVNS_VTN30vCmZpJceD4V3An;database=defaultdb;";
var serverVersion = ServerVersion.AutoDetect(connectionString);

builder.Services.AddDbContext<PlayOfferContext>(options =>
    options.UseMySql(connectionString, serverVersion)
);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
