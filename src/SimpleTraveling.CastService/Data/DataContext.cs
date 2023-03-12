using MongoDB.Driver;

using SimpleTraveling.Abstractions;

namespace SimpleTraveling.CostService.Data;

public class DataContext
{
    public DataContext(IConfiguration configuration)
    {
        Client = new MongoClient(configuration.GetConnectionString("mongodb"));
        Database = Client.GetDatabase(configuration.GetConnectionString("mongodb_database"));
        Bills = Database.GetCollection<Bills>(nameof(Bills));
        Discounts = Database.GetCollection<Discount>(nameof(Discount));
    }

    public MongoClient Client { get; }
    public IMongoDatabase Database { get; }
    public IMongoCollection<Bills> Bills { get; }
    public IMongoCollection<Discount> Discounts { get; }
}
