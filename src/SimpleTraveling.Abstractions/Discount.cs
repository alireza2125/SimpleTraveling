using System.ComponentModel.DataAnnotations;

using MongoDB.Bson;

namespace SimpleTraveling.Abstractions;

public class DiscountBase
{
    public decimal Value { get; set; }
}

public class Discount : DiscountBase
{
    [Key]
    public ObjectId Id { get; set; }
}
