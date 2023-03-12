using Microsoft.AspNetCore.Mvc.Testing;

namespace SimpleTraveling.Test;

public class Context : IDisposable, IAsyncDisposable
{
    private bool _disposedValue;

    public WebApplicationFactory<CostService.Program> CostService { get; } = new();
    public HttpClient CostServiceClient { get; } = new();
    public WebApplicationFactory<TravelService.Program> TravelService { get; } = new();
    public HttpClient TravelServiceClient { get; }
    public WebApplicationFactory<DriverService.Program> DriverService { get; } = new();
    public HttpClient DriverServiceClient { get; }

    public Context()
    {
        CostServiceClient = CostService.CreateClient();
        TravelServiceClient = TravelService.CreateClient();
        DriverServiceClient = DriverService.CreateClient();

    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsyncCore()
    {
        await CostService.DisposeAsync().ConfigureAwait(false);
        await TravelService.DisposeAsync().ConfigureAwait(false);
        await DriverService.DisposeAsync().ConfigureAwait(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                CostService.Dispose();
                TravelService.Dispose();
                DriverService.Dispose();
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
