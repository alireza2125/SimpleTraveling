using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleTraveling.Abstractions;

public class Location
{
    [Key]
    public int Id { get; set; }

    public required string Name { get; set; }

    public float Lat { get; set; }
    public float Lng { get; set; }

    [JsonIgnore]
    public List<Travel> OriginTravels { get; } = new();
    [JsonIgnore]
    public List<Travel> DestinationTravels { get; } = new();
}
