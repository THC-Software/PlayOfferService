using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlayOfferService.Domain;
using PlayOfferService.Domain.Repositories;
using PlayOfferService.Repositories;

namespace PlayOfferService.Tests;

public class WebAppFactory(string readDbConnectionString, string writeDbConnectionString) : WebApplicationFactory<Startup>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(ReplaceDbContext);
    }
    
    private void ReplaceDbContext(IServiceCollection services)
    {
        var existingDbContextRegistration = services.Where(d => 
            d.ServiceType == typeof(DbContextOptions<DbReadContext>) || d.ServiceType == typeof(DbContextOptions<DbWriteContext>)
        ).ToList();

        foreach (var existingDbContext in existingDbContextRegistration)
        {
            services.Remove(existingDbContext); 
        }

        services.AddDbContext<DbReadContext>(options =>
            options.UseNpgsql(readDbConnectionString));
        services.AddDbContext<DbWriteContext>(options =>
            options.UseNpgsql(writeDbConnectionString));
    }
}