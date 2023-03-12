using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;

using SimpleTraveling.Abstractions;
using SimpleTraveling.Remote;

namespace SimpleTraveling.TravelService.Remote;

public class TravelRemote : RemoteBase<TravelRemote>
{
    public TravelRemote(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async IAsyncEnumerable<Travel> GetAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var response = await Client.GetAsync("/api/travels", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            yield break;
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        await foreach (var item in JsonSerializer
            .DeserializeAsyncEnumerable<Travel>(stream, cancellationToken: cancellationToken)
            .WithCancellation(cancellationToken))
        {
            if (item is null)
                continue;

            yield return item;
        }
    }

    public async ValueTask<Travel?> GetAsync(int id, CancellationToken cancellationToken = default) =>
        await Client.GetFromJsonAsync<Travel>($"/api/travels/{id}", cancellationToken).ConfigureAwait(false);

    public async ValueTask<bool> RemoveAsync(int id, CancellationToken cancellationToken = default)
    {
        using var response = await Client
            .DeleteAsync($"/api/travels/{id}", cancellationToken)
            .ConfigureAwait(false);
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<Travel?> UpdateAsync(Travel travel, CancellationToken cancellationToken = default)
    {
        using var response = await Client
            .PutAsJsonAsync($"/api/travels", travel, cancellationToken)
            .ConfigureAwait(false);
        return !response.IsSuccessStatusCode
            ? null
            : await response.Content.ReadFromJsonAsync<Travel>(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Travel?> CreateAsync(TravelBase travelBase, CancellationToken cancellationToken = default)
    {
        using var response = await Client
            .PostAsJsonAsync($"/api/travels", travelBase, cancellationToken)
            .ConfigureAwait(false);
        return !response.IsSuccessStatusCode
            ? null
            : await response.Content.ReadFromJsonAsync<Travel>(cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
