using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

using SimpleTraveling.Abstractions;
using SimpleTraveling.CostService.Data;
using SimpleTraveling.CostService.Services;
using SimpleTraveling.DriverService.Remote;
using SimpleTraveling.TravelService.Remote;

namespace SimpleTraveling.CostService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillsController : ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly AmountService _amountService;
    private readonly DiscountService _discountService;
    private readonly PassengerRemote _passengerRemote;
    private readonly TravelRemote _travelRemote;
    private readonly DriverRemote _driverRemote;

    public BillsController(
        DataContext dataContext,
        AmountService amountService,
        DiscountService discountService,
        PassengerRemote passengerRemote,
        TravelRemote travelRemote,
        DriverRemote driverRemote)
    {
        _dataContext = dataContext;
        _amountService = amountService;
        _discountService = discountService;
        _passengerRemote = passengerRemote;
        _travelRemote = travelRemote;
        _driverRemote = driverRemote;
    }

    [HttpPost]
    public async ValueTask<IActionResult> CreateAsync(BillsBase billsBase, CancellationToken cancellationToken = default)
    {
        var passenger = await _passengerRemote.GetAsync(billsBase.PassengerId, cancellationToken).ConfigureAwait(false);
        if (passenger is null)
        {
            ModelState.AddModelError<BillsBase>(x => x.PassengerId, "not found");
            return BadRequest(ModelState);
        }

        var travel = await _travelRemote.GetAsync(billsBase.TravelId, cancellationToken).ConfigureAwait(false);
        if (travel is null)
        {
            ModelState.AddModelError<BillsBase>(x => x.TravelId, "not found");
            return BadRequest(ModelState);
        }

        var discount = await _dataContext.Discounts.AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == billsBase.DiscountId, cancellationToken)
            .ConfigureAwait(false);

        Bills bills = new()
        {
            TravelId = billsBase.TravelId,
            DiscountId = billsBase.DiscountId,
            PassengerId = billsBase.PassengerId,
            Amount = _amountService.Calucate(travel) - _discountService.Calucate(discount)
        };
        await _dataContext.Bills.InsertOneAsync(bills, null, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetAsync), new { bills.Id, cancellationToken }, bills);
    }

    [HttpPut]
    public async ValueTask<IActionResult> UpdateAsync(Bills bills, CancellationToken cancellationToken = default)
    {
        bills.Passenger = await _passengerRemote.GetAsync(bills.PassengerId, cancellationToken).ConfigureAwait(false);
        if (bills.Passenger is null)
        {
            ModelState.AddModelError<Bills>(x => x.PassengerId, "not found");
            return BadRequest(ModelState);
        }

        bills.Travel = await _travelRemote.GetAsync(bills.TravelId, cancellationToken).ConfigureAwait(false);
        if (bills.Travel is null)
        {
            ModelState.AddModelError<Bills>(x => x.TravelId, "not found");
            return BadRequest(ModelState);
        }

        switch (bills.Status)
        {
            case BillsStatus.Paid:
                if (bills.Travel.Passengers.Any(x => x.Id == bills.Passenger.Id))
                    break;
                bills.Travel.Passengers.Add(bills.Passenger);
                await _travelRemote.UpdateAsync(bills.Travel, cancellationToken).ConfigureAwait(false);
                break;
            case BillsStatus.Waiting:
            case BillsStatus.None:
            default:
                var passenger = bills.Travel.Passengers.FirstOrDefault(x => x.Id == bills.Passenger.Id);
                if (passenger is null)
                    break;
                bills.Travel.Passengers.Remove(passenger);
                break;
        }

        var result = await _dataContext.Bills
            .ReplaceOneAsync(x => x.Id == bills.Id, bills, cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.IsAcknowledged ? AcceptedAtAction(nameof(GetAsync), new { bills.Id, cancellationToken }, bills) : NotFound();
    }

    [HttpDelete("{id}")]
    public async ValueTask<IActionResult> RemoveAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        var result = await _dataContext.Bills
            .DeleteOneAsync(x => x.Id == id, cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.IsAcknowledged ? NoContent() : NotFound();
    }

    [HttpGet("{id}")]
    public async ValueTask<ActionResult<Bills>> GetAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        var result = await _dataContext.Bills.AsQueryable().
            FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);
        return result is null ? NotFound() : result;
    }

    [HttpGet]
    public IAsyncEnumerable<Bills> GetAsync(int skip = 0, [Range(25, 100)] int take = 25) =>
        _dataContext.Bills.AsQueryable().Skip(skip).Take(take).ToAsyncEnumerable();
}
