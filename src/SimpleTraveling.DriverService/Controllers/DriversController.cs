using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

using SimpleTraveling.Abstractions;
using SimpleTraveling.DriverService.Data;

namespace SimpleTraveling.DriverService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DriversController : ControllerBase
{
    private readonly DataContext _context;

    public DriversController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IAsyncEnumerable<Driver> GetAsync() => _context.Driver.AsNoTracking().AsAsyncEnumerable();

    [HttpGet("{id:int}")]
    public async ValueTask<ActionResult<Driver>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var driver = await _context.Driver.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return driver == null ? NotFound() : driver;
    }

    public async Task<IActionResult> UpdateAsync(Driver driver, CancellationToken cancellationToken = default)
    {
        if (await _context.Driver.AllAsync(x => x.Id == driver.Id, cancellationToken).ConfigureAwait(false))
        {
            ModelState.AddModelError<Driver>(x => x.Id, "not found");
            return NotFound(ModelState);
        }

        _context.Update(driver);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Driver>> CreateAsync(Driver driver, CancellationToken cancellationToken = default)
    {
        await _context.Driver.AddAsync(driver, cancellationToken).ConfigureAwait(false);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetAsync), new { id = driver.Id, cancellationToken }, driver);
    }

    [HttpDelete("{id:int}")]
    public async ValueTask<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var driver = await _context.Driver.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (driver == null)
        {
            return NotFound();
        }

        _context.Driver.Remove(driver);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return NoContent();
    }
}
