using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;

using SimpleTraveling.Abstractions;
using SimpleTraveling.Remote;

namespace SimpleTraveling.TravelService.Remote;

public class DiscountsRemote : RemoteBase<DiscountsRemote>
{
    public DiscountsRemote(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async IAsyncEnumerable<Discount> GetAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var response = await Client.GetAsync("/api/discounts", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            yield break;
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        await foreach (var item in JsonSerializer
            .DeserializeAsyncEnumerable<Discount>(stream, cancellationToken: cancellationToken)
            .WithCancellation(cancellationToken))
        {
            if (item is null)
                continue;

            yield return item;
        }
    }

    public async ValueTask<Discount?> GetAsync(int id, CancellationToken cancellationToken = default) =>
        await Client.GetFromJsonAsync<Discount>($"/api/discounts/{id}", cancellationToken).ConfigureAwait(false);

    public async ValueTask<bool> RemoveAsync(int id, CancellationToken cancellationToken = default)
    {
        using var response = await Client
            .DeleteAsync($"/api/discounts/{id}", cancellationToken)
            .ConfigureAwait(false);
        return response.IsSuccessStatusCode;
    }

    public async ValueTask<Discount?> UpdateAsync(Discount travel, CancellationToken cancellationToken = default)
    {
        using var response = await Client
            .PutAsJsonAsync($"/api/discounts", travel, cancellationToken)
            .ConfigureAwait(false);
        return !response.IsSuccessStatusCode
            ? null
            : await response.Content.ReadFromJsonAsync<Discount>(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Discount?> CreateAsync(Discount Discount, CancellationToken cancellationToken = default)
    {
        using var response = await Client
            .PostAsJsonAsync($"/api/discounts", Discount, cancellationToken)
            .ConfigureAwait(false);
        return !response.IsSuccessStatusCode
            ? null
            : await response.Content.ReadFromJsonAsync<Discount>(cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
