using Microsoft.EntityFrameworkCore;

using SimpleTraveling.Abstractions;

namespace SimpleTraveling.TravelService.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Passenger> Passengers => Set<Passenger>();
    public DbSet<Travel> Travels => Set<Travel>();
    public DbSet<Location> Locations => Set<Location>();
}
