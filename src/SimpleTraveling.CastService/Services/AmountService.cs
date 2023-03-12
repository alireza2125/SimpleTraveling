using SimpleTraveling.Abstractions;

namespace SimpleTraveling.CostService.Services;

public class AmountService
{
    public decimal Calucate(Travel travel)
    {
        _ = travel;
        return Random.Shared.Next(10000,80000);
    }
}
