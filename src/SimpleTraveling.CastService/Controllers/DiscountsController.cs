using System.ComponentModel.DataAnnotations;
using System.Net;

using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

using SimpleTraveling.Abstractions;
using SimpleTraveling.CostService.Data;

namespace SimpleTraveling.CostService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscountsController : ControllerBase
{
    private readonly DataContext _dataContext;

    public DiscountsController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async ValueTask<IActionResult> Create(DiscountBase discount, CancellationToken cancellationToken = default)
    {
        Discount document = new Discount() { Value = discount.Value };
        await _dataContext.Discounts.InsertOneAsync(document, null, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(Get), new { document.Id, cancellationToken }, discount);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> Update(Discount discount, CancellationToken cancellationToken = default)
    {
        var result = await _dataContext.Discounts
            .ReplaceOneAsync(x => x.Id == discount.Id, discount, cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.IsAcknowledged ? AcceptedAtAction(nameof(Get), new { discount.Id, cancellationToken }, discount) : NotFound();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> Remove(ObjectId id, CancellationToken cancellationToken = default)
    {
        var result = await _dataContext.Discounts
            .DeleteOneAsync(x => x.Id == id, cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.IsAcknowledged ? NoContent() : NotFound();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<Discount>> Get(ObjectId id, CancellationToken cancellationToken = default)
    {
        var result = await _dataContext.Discounts.AsQueryable().
            FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);
        return result is null ? NotFound() : result;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IAsyncEnumerable<Discount> Get(int skip = 0, [Range(25, 100)] int take = 25) =>
        _dataContext.Discounts.AsQueryable().Skip(skip).Take(take).ToAsyncEnumerable();
}
