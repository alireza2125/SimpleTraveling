using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleTraveling.Abstractions;

public class LocationBase
{
    public string Name { get; set; } = string.Empty;

    public float Lat { get; set; }
    public float Lng { get; set; }
}

public class Location : LocationBase
{
    [Key]
    public int Id { get; set; }

    [JsonIgnore]
    public List<Travel> OriginTravels { get; } = new();
    [JsonIgnore]
    public List<Travel> DestinationTravels { get; } = new();
}
