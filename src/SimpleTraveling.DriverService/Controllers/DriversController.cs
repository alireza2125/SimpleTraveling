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
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IAsyncEnumerable<Driver> Get() => _context.Driver.AsNoTracking().AsAsyncEnumerable();

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<Driver>> Get(int id, CancellationToken cancellationToken = default)
    {
        var driver = await _context.Driver.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return driver == null ? NotFound() : driver;
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Driver driver, CancellationToken cancellationToken = default)
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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Driver>> Create(DriverBase driver, CancellationToken cancellationToken = default)
    {
        var entity = new Driver
        {
            CarBrand = driver.CarBrand,
            CarModel = driver.CarModel,
            Firstname = driver.Firstname,
            Lastname = driver.Lastname,
            LicensePlate = driver.LicensePlate,
            PersonalId = driver.PersonalId,
            PhoneNumber = driver.PhoneNumber
        };

        await _context.Driver.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(Get), new { id = entity.Id, cancellationToken }, entity);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
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
