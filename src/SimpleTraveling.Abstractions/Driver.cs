using System.ComponentModel.DataAnnotations;

namespace SimpleTraveling.Abstractions;

public class DriverBase
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

    [Required]
    public string CarModel { get; set; } = string.Empty;
    [Required]
    public string CarBrand { get; set; } = string.Empty;
    [Required]
    public string LicensePlate { get; set; } = string.Empty;
}

public class Driver : DriverBase
{
    [Key]
    public int Id { get; set; }
}
