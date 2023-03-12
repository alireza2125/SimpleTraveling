using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SimpleTraveling.Abstractions;
using SimpleTraveling.TravelService.Data;

namespace SimpleTraveling.TravelService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly DataContext _dataContext;

    public LocationsController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> CreateAsync(Location location, CancellationToken cancellationToken = default)
    {
        await _dataContext.AddAsync(location, cancellationToken).ConfigureAwait(false);
        await _dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetAsync), new { location.Id, cancellationToken }, location);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> UpdateAsync(Location locations, CancellationToken cancellationToken = default)
    {

        if (await _dataContext.Locations.AllAsync(x => x.Id != locations.Id, cancellationToken).ConfigureAwait(false))
            return NotFound();

        locations.Id = default;
        _dataContext.Update(locations);
        await _dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return AcceptedAtAction(nameof(GetAsync), new { locations.Id, cancellationToken }, locations);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var lcoations = await _dataContext.Locations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);
        if (lcoations is null)
            return NotFound();

        _dataContext.Remove(lcoations);
        await _dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    [HttpGet]
    public IAsyncEnumerable<Location> GetAsync(int skip = 0, [Range(25, 100)] int take = 25) =>
        _dataContext.Locations.AsNoTracking().Skip(skip).Take(take).AsAsyncEnumerable();

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<Location>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var location = await _dataContext.Locations.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);
        return location is null ? NotFound() : location;
    }
}
