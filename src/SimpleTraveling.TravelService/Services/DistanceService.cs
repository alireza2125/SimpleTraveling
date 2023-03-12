using SimpleTraveling.Abstractions;

namespace SimpleTraveling.TravelService.Services;

public class DistanceService
{
    public ValueTask<double> Get(Location from, Location to, CancellationToken cancellationToken = default)
    {
        _ = from;
        _ = to;
        _ = cancellationToken;
        return new(Random.Shared.Next(6, 15) + Random.Shared.NextDouble());
    }
}
