using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SimpleTraveling.Abstractions;
using SimpleTraveling.TravelService.Data;

namespace SimpleTraveling.TravelService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TravelsController : ControllerBase
{
    private readonly DataContext _dataContext;

    public TravelsController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async ValueTask<IActionResult> Create(TravelBase travel, CancellationToken cancellationToken = default)
    {
        var entity = new Travel
        {
            DestinationId = travel.DestinationId,
            DriverId = travel.DriverId,
            OriginId = travel.OriginId
        };
        await _dataContext.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        await _dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(Get), new { entity.Id, cancellationToken }, entity);
    }

    [HttpPut]
    public async ValueTask<IActionResult> Update(Travel travel, CancellationToken cancellationToken = default)
    {
        _dataContext.Update(travel);
        await _dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return AcceptedAtAction(nameof(Get), new { travel.Id, cancellationToken }, travel);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var travel = await _dataContext.Travels.FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);
        if (travel is null)
            return NotFound();

        _dataContext.Remove(travel);
        await _dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IAsyncEnumerable<Travel> Get(int skip = 0, [Range(25, 100)] int take = 25) =>
        _dataContext.Travels
            .Include(x => x.Passengers)
            .Include(x => x.Origin)
            .Include(x => x.Destination)
            .AsNoTracking().Skip(skip).Take(take).AsAsyncEnumerable();

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<Travel>> Get(int id, CancellationToken cancellationToken = default)
    {
        var travel = await _dataContext.Travels
            .Include(x => x.Passengers)
            .Include(x => x.Origin)
            .Include(x => x.Destination)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);
        return travel is null ? NotFound() : travel;
    }
}
