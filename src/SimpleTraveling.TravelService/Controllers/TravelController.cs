using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SimpleTraveling.Abstractions;
using SimpleTraveling.TravelService.Data;

namespace SimpleTraveling.TravelService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TravelController : ControllerBase
{
    private readonly DataContext _dataContext;

    public TravelController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpPost]
    public async ValueTask<IActionResult> CreateAsync(Travel travel, CancellationToken cancellationToken = default)
    {
        await _dataContext.AddAsync(travel, cancellationToken).ConfigureAwait(false);
        await _dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetAsync), new { travel.Id, cancellationToken }, travel);
    }

    [HttpPut]
    public async ValueTask<IActionResult> UpdateAsync(Travel travel, CancellationToken cancellationToken = default)
    {
        _dataContext.Update(travel);
        await _dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return AcceptedAtAction(nameof(GetAsync), new { travel.Id, cancellationToken }, travel);
    }

    [HttpDelete("{id:int}")]
    public async ValueTask<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var travel = await _dataContext.Travels.FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);
        if (travel is null)
            return NotFound();

        _dataContext.Remove(travel);
        await _dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    [HttpGet]
    public IAsyncEnumerable<Travel> GetAsync(int skip = 0, [Range(25, 100)] int take = 25) =>
        _dataContext.Travels
            .Include(x => x.Passengers)
            .Include(x => x.Driver)
            .AsNoTracking().Skip(skip).Take(take).AsAsyncEnumerable();

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async ValueTask<ActionResult<Travel>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var travel = await _dataContext.Travels
            .Include(x => x.Passengers)
            .Include(x => x.Driver)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);
        return travel is null ? NotFound() : travel;
    }
}
