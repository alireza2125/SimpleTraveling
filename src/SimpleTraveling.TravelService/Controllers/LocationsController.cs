﻿using System.ComponentModel.DataAnnotations;

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
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async ValueTask<IActionResult> Create(LocationBase location, CancellationToken cancellationToken = default)
    {
        var entity = new Location
        {
            Name = location.Name,
            Lat = location.Lat,
            Lng = location.Lng
        };

        await _dataContext.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        await _dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(Get), new { entity.Id, cancellationToken }, entity);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> Update(Location locations, CancellationToken cancellationToken = default)
    {

        if (await _dataContext.Locations.AllAsync(x => x.Id != locations.Id, cancellationToken).ConfigureAwait(false))
            return NotFound();

        locations.Id = default;
        _dataContext.Update(locations);
        await _dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return AcceptedAtAction(nameof(Get), new { locations.Id, cancellationToken }, locations);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var lcoations = await _dataContext.Locations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);
        if (lcoations is null)
            return NotFound();

        _dataContext.Remove(lcoations);
        await _dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IAsyncEnumerable<Location> Get(int skip = 0, [Range(25, 100)] int take = 25) =>
        _dataContext.Locations.AsNoTracking().Skip(skip).Take(take).AsAsyncEnumerable();

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<Location>> Get(int id, CancellationToken cancellationToken = default)
    {
        var location = await _dataContext.Locations.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);
        return location is null ? NotFound() : location;
    }
}
