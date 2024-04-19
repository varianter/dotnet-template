using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public class DatabaseContext(IOptions<InfrastructureConfig> config) : DbContext, IUnitOfWork
{
    public DbSet<Forecast> Forecasts { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(config.Value.ConnectionString);
    }
}