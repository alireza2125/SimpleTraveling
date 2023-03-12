using Microsoft.EntityFrameworkCore;

using SimpleTraveling.Abstractions;

namespace SimpleTraveling.DriverService.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<Driver> Driver { get; set; } = default!;
}
