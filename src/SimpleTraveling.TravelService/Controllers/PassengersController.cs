using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SimpleTraveling.Abstractions;

using SimpleTraveling.TravelService.Data;

namespace SimpleTraveling.TravelService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PassengersController : ControllerBase
{
    private readonly DataContext _dataContext;

    public PassengersController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> CreateAsync(Passenger passenger, CancellationToken cancellationToken = default)
    {
        await _dataContext.AddAsync(passenger, cancellationToken).ConfigureAwait(false);
        await _dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetAsync), new { passenger.Id, cancellationToken }, passenger);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> UpdateAsync(Passenger passenger, CancellationToken cancellationToken = default)
    {

        if (await _dataContext.Passengers.AllAsync(x => x.Id != passenger.Id, cancellationToken).ConfigureAwait(false))
            return NotFound();

        passenger.Id = default;
        _dataContext.Update(passenger);
        await _dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return AcceptedAtAction(nameof(GetAsync), new { passenger.Id, cancellationToken }, passenger);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var passenger = await _dataContext.Passengers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);
        if (passenger is null)
            return NotFound();

        _dataContext.Remove(passenger);
        await _dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    [HttpGet]
    public IAsyncEnumerable<Passenger> GetAsync(int skip = 0, [Range(25, 100)] int take = 25) =>
        _dataContext.Passengers.AsNoTracking().Skip(skip).Take(take).AsAsyncEnumerable();

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<Passenger>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var passenger = await _dataContext.Passengers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);
        return passenger is null ? NotFound() : passenger;
    }
}
