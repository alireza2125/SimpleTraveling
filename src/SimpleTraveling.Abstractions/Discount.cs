using System.ComponentModel.DataAnnotations;

using MongoDB.Bson;

namespace SimpleTraveling.Abstractions;

public class Discount
{
    [Key]
    public ObjectId Id { get; set; }

    public decimal Value { get; set; }
}
