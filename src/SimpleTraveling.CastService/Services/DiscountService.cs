using SimpleTraveling.Abstractions;

namespace SimpleTraveling.CostService.Services;

public class DiscountService
{
    public decimal Calucate(Discount? discount)
    {
        if (discount == null)
            return 0m;

        _ = discount;
        return Random.Shared.Next(2000, 6000);
    }
}
