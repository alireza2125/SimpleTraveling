using System.Globalization;
using System.Net.Http.Json;

using MongoDB.Bson;

using SimpleTraveling.Abstractions;

using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace SimpleTraveling.Test;

public class DefaultSenarioUnitTest : IClassFixture<Context>
{
    private readonly Context _context;
    private readonly ITestOutputHelper _testOutputHelper;

    public DefaultSenarioUnitTest(Context context, ITestOutputHelper testOutputHelper)
    {
        _context = context;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Create()
    {
        // create item
        var discount = await CreateDiscountAsync();
        // create location1
        var location1 = await CreateLocationAsync();
        var location2 = await CreateLocationAsync();
        // create driver
        var driver = await CreateDriverAsync();
        // create travel
        var travel = await CreateTravelAsync(driver.Id, location1.Id, location2.Id);
        // create passengers
        var passenger = await CreatePassengerAsync();
        // create bills
        var bills = await CreateBillsAsync(travel.Id, passenger.Id, discount.Id);
        // apply bills
        bills.Status = BillsStatus.Paid;
        await UpdateBillsAsync(bills);
    }

    private async Task<Discount> CreateDiscountAsync()
    {
        Discount item = new()
        {
            Value = 0.05m
        };
        using var response = await _context.CostServiceClient.PostAsJsonAsync("/api/discounts", item).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        item = (await response.Content.ReadFromJsonAsync<Discount>().ConfigureAwait(false))!;
        return _testOutputHelper.WriteLineObject(item);
    }

    public async Task<Location> CreateLocationAsync()
    {
        Location item = new()
        {
            Name = Guid.NewGuid().ToString()
        };
        using var response = await _context.CostServiceClient.PostAsJsonAsync("/api/locations", item).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        item = (await response.Content.ReadFromJsonAsync<Location>().ConfigureAwait(false))!;
        return _testOutputHelper.WriteLineObject(item);
    }

    public async Task<Driver> CreateDriverAsync()
    {
        Driver item = new()
        {
            Firstname = Guid.NewGuid().ToString(),
            Lastname = Guid.NewGuid().ToString(),
            CarBrand = Guid.NewGuid().ToString(),
            CarModel = Guid.NewGuid().ToString(),
            LicensePlate = Guid.NewGuid().ToString(),
            PersonalId = Random.Shared.NextInt64(1000000000, 9999999999).ToString(CultureInfo.CurrentCulture),
            PhoneNumber = Random.Shared.NextInt64(1000000000, 9999999999).ToString(CultureInfo.CurrentCulture),
        };
        using var response = await _context.DriverServiceClient.PostAsJsonAsync("/api/drivers", item).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        item = (await response.Content.ReadFromJsonAsync<Driver>().ConfigureAwait(false))!;
        return _testOutputHelper.WriteLineObject(item);
    }

    public async Task<Passenger> CreatePassengerAsync()
    {
        Passenger item = new()
        {
            Firstname = Guid.NewGuid().ToString(),
            Lastname = Guid.NewGuid().ToString(),
            PersonalId = Random.Shared.NextInt64(1000000000, 9999999999).ToString(CultureInfo.CurrentCulture),
            PhoneNumber = Random.Shared.NextInt64(1000000000, 9999999999).ToString(CultureInfo.CurrentCulture),
        };
        using var response = await _context.DriverServiceClient.PostAsJsonAsync("/api/passengers", item).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        item = (await response.Content.ReadFromJsonAsync<Passenger>().ConfigureAwait(false))!;
        return _testOutputHelper.WriteLineObject(item);
    }

    public async Task<Travel> CreateTravelAsync(int driverId, int originId, int destinationId)
    {
        TravelBase item = new()
        {
            DriverId = driverId,
            OriginId = originId,
            DestinationId = destinationId
        };
        using var response = await _context.DriverServiceClient.PostAsJsonAsync("/api/passengers", item).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return _testOutputHelper.WriteLineObject((await response.Content.ReadFromJsonAsync<Travel>().ConfigureAwait(false))!);
    }

    public async Task<Bills> CreateBillsAsync(int travelId, int passengerId, ObjectId? discountId = null)
    {
        BillsBase item = new()
        {
            TravelId = travelId,
            PassengerId = passengerId,
            DiscountId = discountId,
        };
        using var response = await _context.DriverServiceClient.PostAsJsonAsync("/api/passengers", item).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return _testOutputHelper.WriteLineObject((await response.Content.ReadFromJsonAsync<Bills>().ConfigureAwait(false))!);
    }

    public async Task<Bills> UpdateBillsAsync(Bills bills)
    {

        using var response = await _context.DriverServiceClient.PutAsJsonAsync("/api/passengers", bills).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return _testOutputHelper.WriteLineObject((await response.Content.ReadFromJsonAsync<Bills>().ConfigureAwait(false))!);
    }
}
