using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace SimpleTraveling.Abstractions;

public class TravelBase
{
    [Required]
    public int DriverId { get; set; }
    [Required]
    public int OriginId { get; set; }
    [Required]
    public int DestinationId { get; set; }
}

[EntityTypeConfiguration(typeof(TravelConfiguration))]
public class Travel : TravelBase
{
    [Key]
    public int Id { get; set; }

    public TravelStatus Status { get; set; } = TravelStatus.None;

    [NotMapped]
    public Driver? Driver { get; set; }

    public Location? Origin { get; set; }

    public Location? Destination { get; set; }

    [NotMapped]
    public double Distance { get; set; }

    public List<Passenger> Passengers { get; } = new();

}

public class TravelConfiguration : IEntityTypeConfiguration<Travel>
{
    public void Configure(EntityTypeBuilder<Travel> builder)
    {
        builder.HasOne(x => x.Origin)
             .WithMany(x => x.OriginTravels)
             .HasForeignKey(x => x.OriginId)
             .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Destination)
             .WithMany(x => x.DestinationTravels)
             .HasForeignKey(x => x.DestinationId)
             .OnDelete(DeleteBehavior.Restrict);
    }
}
