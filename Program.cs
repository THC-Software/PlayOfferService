using Microsoft.EntityFrameworkCore;
using PlayOfferService.Repositories;

var builder = WebApplication.CreateBuilder(args);
var connectionString = "mysql://doadmin:AVNS_VTN30vCmZpJceD4V3An@playofferservice-db-do-user-14755325-0.c.db.ondigitalocean.com:25060/defaultdb?ssl-mode=REQUIRED";
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
