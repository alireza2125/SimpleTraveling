using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SimpleTraveling.Abstractions;

public class PassengerBase
{
    [Required]
    public string Firstname { get; set; } = string.Empty;

    [Required]
    public string Lastname { get; set; } = string.Empty;

    [Required]
    [RegularExpression("\\d{3}-?\\d{3}-?\\d{4}")]
    public string PersonalId { get; set; } = string.Empty;

    [Phone]
    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
}

public class Passenger : PassengerBase
{
    [Key]
    public int Id { get; set; }

    [JsonIgnore]
    public List<Travel> Travels { get; } = new();
}
