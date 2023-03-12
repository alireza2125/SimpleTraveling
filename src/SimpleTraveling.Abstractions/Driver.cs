using System.ComponentModel.DataAnnotations;

namespace SimpleTraveling.Abstractions;

public class Driver : Passenger
{
    [Required]
    public required string CarModel { get; set; }
    [Required]
    public required string CarBrand { get; set; }
    [Required]
    public required string LicensePlate { get; set; }
}
