using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

using SimpleTraveling.Abstractions;
using SimpleTraveling.CostService.Data;

namespace SimpleTraveling.CostService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscountController : ControllerBase
{
    private readonly DataContext _dataContext;

    public DiscountController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpPost]
    public async ValueTask<IActionResult> CreateAsync(Discount discount, CancellationToken cancellationToken = default)
    {
        await _dataContext.Discounts.InsertOneAsync(discount, null, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetAsync), new { discount.Id, cancellationToken }, discount);
    }

    [HttpPut]
    public async ValueTask<IActionResult> UpdateAsync(Discount discount, CancellationToken cancellationToken = default)
    {
        var result = await _dataContext.Discounts
            .ReplaceOneAsync(x => x.Id == discount.Id, discount, cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.IsAcknowledged ? AcceptedAtAction(nameof(GetAsync), new { discount.Id, cancellationToken }, discount) : NotFound();
    }

    [HttpDelete("{id}")]
    public async ValueTask<IActionResult> RemoveAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        var result = await _dataContext.Discounts
            .DeleteOneAsync(x => x.Id == id, cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.IsAcknowledged ? NoContent() : NotFound();
    }

    [HttpGet("{id}")]
    public async ValueTask<ActionResult<Discount>> GetAsync(ObjectId id, CancellationToken cancellationToken = default)
    {
        var result = await _dataContext.Discounts.AsQueryable().
            FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);
        return result is null ? NotFound() : result;
    }

    [HttpGet]
    public IAsyncEnumerable<Discount> GetAsync(int skip = 0, [Range(25, 100)] int take = 25) =>
        _dataContext.Discounts.AsQueryable().Skip(skip).Take(take).ToAsyncEnumerable();
}
