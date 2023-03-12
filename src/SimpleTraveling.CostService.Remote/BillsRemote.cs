using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;

using SimpleTraveling.Abstractions;
using SimpleTraveling.Remote;

namespace SimpleTraveling.TravelService.Remote;

public class BillsRemote : RemoteBase<BillsRemote>
{
    public BillsRemote(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async IAsyncEnumerable<Bills> GetAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var response = await Client.GetAsync("/api/bills", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            yield break;
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        await foreach (var item in JsonSerializer
            .DeserializeAsyncEnumerable<Bills>(stream, cancellationToken: cancellationToken)
            .WithCancellation(cancellationToken))
        {
            if (item is null)
                continue;

            yield return item;
        }
    }

    public async ValueTask<Bills?> GetAsync(int id, CancellationToken cancellationToken = default) =>
        await Client.GetFromJsonAsync<Bills>($"/api/bills/{id}", cancellationToken).ConfigureAwait(false);

    public async ValueTask<bool> RemoveAsync(int id, CancellationToken cancellationToken = default)
    {
        using var response = await Client
            .DeleteAsync($"/api/bills/{id}", cancellationToken)
            .ConfigureAwait(false);
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<Bills?> UpdateAsync(Bills travel, CancellationToken cancellationToken = default)
    {
        using var response = await Client
            .PutAsJsonAsync($"/api/bills", travel, cancellationToken)
            .ConfigureAwait(false);
        return !response.IsSuccessStatusCode
            ? null
            : await response.Content.ReadFromJsonAsync<Bills>(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Bills?> CreateAsync(Bills Bills, CancellationToken cancellationToken = default)
    {
        using var response = await Client
            .PostAsJsonAsync($"/api/bills", Bills, cancellationToken)
            .ConfigureAwait(false);
        return !response.IsSuccessStatusCode
            ? null
            : await response.Content.ReadFromJsonAsync<Bills>(cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
