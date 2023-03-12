using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;

using SimpleTraveling.Test.Helpers;

namespace SimpleTraveling.Test;

public class Context : IDisposable, IAsyncDisposable
{
    public static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNameCaseInsensitive = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = {
            new ObjectIdConverter()
        }
    };

    private bool _disposedValue;

    //public WebApplicationFactory<CostService.Program> CostService { get; } = null!;
    //public WebApplicationFactory<TravelService.Program> TravelService { get; } = null!;
    //public WebApplicationFactory<DriverService.Program> DriverService { get; } = null!;

    public HttpClient CostServiceClient { get; }
    public HttpClient TravelServiceClient { get; }
    public HttpClient DriverServiceClient { get; }

    public Context()
    {

        //CostService = new();
        //TravelService = new();
        //DriverService = new();

        //CostServiceClient = CostService.CreateClient();
        //TravelServiceClient = TravelService.CreateClient();
        //DriverServiceClient = DriverService.CreateClient();

        CostServiceClient = new HttpClient() { BaseAddress = new("http://localhost:4173/") };
        TravelServiceClient = new HttpClient() { BaseAddress = new("http://localhost:6234/") };
        DriverServiceClient = new HttpClient() { BaseAddress = new("http://localhost:5234/") };
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsyncCore()
    {
        await ValueTask.CompletedTask;
        //if (CostService is not null)
        //    await CostService.DisposeAsync().ConfigureAwait(false);
        //if (TravelService is not null)
        //    await TravelService.DisposeAsync().ConfigureAwait(false);
        //if (DriverService is not null)
        //    await DriverService.DisposeAsync().ConfigureAwait(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                //CostService?.Dispose();
                //TravelService?.Dispose();
                //DriverService?.Dispose();

                CostServiceClient.Dispose();
                TravelServiceClient.Dispose();
                DriverServiceClient.Dispose();
            }

            _disposedValue = true;
        }
    }

    ~Context()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
