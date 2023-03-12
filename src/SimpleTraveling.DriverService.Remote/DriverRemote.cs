using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;

using SimpleTraveling.Abstractions;

using SimpleTraveling.Remote;

namespace SimpleTraveling.DriverService.Remote;

public class DriverRemote : RemoteBase<DriverRemote>
{
    public DriverRemote(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async IAsyncEnumerable<Driver> GetAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var response = await Client.GetAsync("/api/drivers", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            yield break;
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        await foreach (var item in JsonSerializer
            .DeserializeAsyncEnumerable<Driver>(stream, cancellationToken: cancellationToken)
            .WithCancellation(cancellationToken))
        {
            if (item is null)
                continue;

            yield return item;
        }
    }

    public async ValueTask<Driver?> GetAsync(int id, CancellationToken cancellationToken = default) =>
        await Client.GetFromJsonAsync<Driver>($"/api/drivers/{id}", cancellationToken).ConfigureAwait(false);
}
