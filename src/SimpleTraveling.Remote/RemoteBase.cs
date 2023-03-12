using Microsoft.Extensions.DependencyInjection;

namespace SimpleTraveling.Remote;
public abstract class RemoteBase<TRemote>
    where TRemote : RemoteBase<TRemote>
{
    protected HttpClient Client { get; }

    protected RemoteBase(IHttpClientFactory httpClientFactory)
    {
        Client = httpClientFactory.CreateClient(typeof(TRemote).GUID.ToString());
    }
}

public record RemoteBuilder(IServiceCollection Services)
{
    public RemoteBuilder Add<TRemote>(Action<HttpClient> action)
     where TRemote : RemoteBase<TRemote>
    {
        Services.AddHttpClient(typeof(TRemote).GUID.ToString(), action);
        Services.AddScoped<TRemote>();
        return this;
    }

    public RemoteBuilder Add<TRemote>(Action<IServiceProvider, HttpClient> action)
     where TRemote : RemoteBase<TRemote>
    {
        Services.AddHttpClient(typeof(TRemote).GUID.ToString(), action);
        Services.AddScoped<TRemote>();
        return this;
    }
}

public static class RemoteExtensions
{
    public static RemoteBuilder AddRemoteService(this IServiceCollection services)
    {
        services.AddHttpClient();
        return new(services);
    }
}
