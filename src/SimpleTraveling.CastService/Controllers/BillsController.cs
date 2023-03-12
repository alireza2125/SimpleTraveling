using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async ValueTask<IActionResult> Create(BillsBase bills, CancellationToken cancellationToken = default)
    {
        var passenger = await _passengerRemote.GetAsync(bills.PassengerId, cancellationToken).ConfigureAwait(false);
        if (passenger is null)
        {
            ModelState.AddModelError<BillsBase>(x => x.PassengerId, "not found");
            return BadRequest(ModelState);
        }

        var travel = await _travelRemote.GetAsync(bills.TravelId, cancellationToken).ConfigureAwait(false);
        if (travel is null)
        {
            ModelState.AddModelError<BillsBase>(x => x.TravelId, "not found");
            return BadRequest(ModelState);
        }

        var discount = await _dataContext.Discounts.AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == bills.DiscountId, cancellationToken)
            .ConfigureAwait(false);

        Bills entity = new()
        {
            TravelId = bills.TravelId,
            DiscountId = bills.DiscountId,
            PassengerId = bills.PassengerId,
            Discount = _discountService.Calucate(discount),
            Amount = _amountService.Calucate(travel),
        };

        entity.FinalAmount = entity.Amount - entity.Discount;
        await _dataContext.Bills.InsertOneAsync(entity, null, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(Get), new { entity.Id, cancellationToken }, entity);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async ValueTask<IActionResult> Update(Bills bills, CancellationToken cancellationToken = default)
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
        return result.IsAcknowledged ? AcceptedAtAction(nameof(Get), new { bills.Id, cancellationToken }, bills) : NotFound();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> Remove(ObjectId id, CancellationToken cancellationToken = default)
    {
        var result = await _dataContext.Bills
            .DeleteOneAsync(x => x.Id == id, cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.IsAcknowledged ? NoContent() : NotFound();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<Bills>> Get(ObjectId id, CancellationToken cancellationToken = default)
    {
        var result = await _dataContext.Bills.AsQueryable().
            FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);
        return result is null ? NotFound() : result;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IAsyncEnumerable<Bills> Get(int skip = 0, [Range(25, 100)] int take = 25) =>
        _dataContext.Bills.AsQueryable().Skip(skip).Take(take).ToAsyncEnumerable();
}
