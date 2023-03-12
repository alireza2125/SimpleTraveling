using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SimpleTraveling.Abstractions;
public class BillsBase
{
    public int TravelId { get; set; }
    public int PassengerId { get; set; }
    public ObjectId? DiscountId { get; set; }
}

public class Bills : BillsBase
{
    [Key]
    public ObjectId Id { get; set; }

    public DateTimeOffset CreateAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset UpdateAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset DeleteAt { get; set; } = DateTimeOffset.Now;

    public BillsStatus Status { get; set; } = BillsStatus.None;

    [BsonIgnore]
    public Travel? Travel { get; set; }

    public decimal Amount { get; set; }

    [BsonIgnore]
    public decimal FinalAmount { get; set; }

    [BsonIgnore]
    public Passenger? Passenger { get; set; }
}
