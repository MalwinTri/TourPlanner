using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TourPlanner.DAL.Postgres;
using Xunit;

public class DatabaseConnectionTests
{
    private readonly ServiceProvider _serviceProvider;

    public DatabaseConnectionTests()
    {
        var services = new ServiceCollection();

        var connectionString = "Host=localhost;Port=5432;Database=tourplannerdb;Username=postgres;Password=postgres";

        services.AddDbContext<TourPlannerDbContext>(options =>
            options.UseNpgsql(connectionString));

        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public void CanConnectToDatabaseAndQueryTours()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TourPlannerDbContext>();

        // Try to connect and query something simple
        var tourCount = context.Tours.Count();  // -> Falls Tabelle existiert

        Assert.True(tourCount >= 0); // kein Fehler => Verbindung okay
    }
}
