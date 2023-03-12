using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SimpleTraveling.Abstractions;

public class Passenger
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Firstname { get; set; }

    [Required]
    public required string Lastname { get; set; }

    [Required]
    [RegularExpression("\\d{3}-?\\d{3}-?\\d{4}")]
    public required string PersonalId { get; set; }

    [Phone]
    [Required]
    public required string PhoneNumber { get; set; }

    [JsonIgnore]
    public List<Travel> Travels { get; } = new();
}
