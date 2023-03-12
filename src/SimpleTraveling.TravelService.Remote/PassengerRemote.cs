using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;

using SimpleTraveling.Abstractions;
using SimpleTraveling.Remote;

namespace SimpleTraveling.TravelService.Remote;

public class PassengerRemote : RemoteBase<PassengerRemote>
{
    public PassengerRemote(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async IAsyncEnumerable<Passenger> GetAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var response = await Client.GetAsync("/api/passengers", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            yield break;
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        await foreach (var item in JsonSerializer
            .DeserializeAsyncEnumerable<Passenger>(stream, cancellationToken: cancellationToken)
            .WithCancellation(cancellationToken))
        {
            if (item is null)
                continue;

            yield return item;
        }
    }

    public async ValueTask<Passenger?> GetAsync(int id, CancellationToken cancellationToken = default) =>
        await Client.GetFromJsonAsync<Passenger>($"/api/passengers/{id}", cancellationToken).ConfigureAwait(false);

    public async ValueTask<bool> RemoveAsync(int id, CancellationToken cancellationToken = default)
    {
        using var response = await Client
            .DeleteAsync($"/api/passengers/{id}", cancellationToken)
            .ConfigureAwait(false);
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<Passenger?> UpdateAsync(Passenger travel, CancellationToken cancellationToken = default)
    {
        using var response = await Client
            .PutAsJsonAsync($"/api/passengers", travel, cancellationToken)
            .ConfigureAwait(false);
        return !response.IsSuccessStatusCode
            ? null
            : await response.Content.ReadFromJsonAsync<Passenger>(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Passenger?> CreateAsync(Passenger passenger, CancellationToken cancellationToken = default)
    {
        using var response = await Client
            .PostAsJsonAsync($"/api/passengers", passenger, cancellationToken)
            .ConfigureAwait(false);
        return !response.IsSuccessStatusCode
            ? null
            : await response.Content.ReadFromJsonAsync<Passenger>(cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
