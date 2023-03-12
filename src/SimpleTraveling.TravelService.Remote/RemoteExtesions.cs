using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SimpleTraveling.Remote;

namespace SimpleTraveling.TravelService.Remote;

public static class RemoteExtesions
{
    public static RemoteBuilder AddPassengersRemote(this RemoteBuilder builder) =>
        builder.Add<PassengerRemote>((provider, options) =>
            options.BaseAddress = new(provider.GetRequiredService<IConfiguration>().GetConnectionString("travelservice")!, UriKind.RelativeOrAbsolute));

    public static RemoteBuilder AddTravelsRemote(this RemoteBuilder builder) =>
        builder.Add<TravelRemote>((provider, options) =>
            options.BaseAddress = new(provider.GetRequiredService<IConfiguration>().GetConnectionString("travelservice")!, UriKind.RelativeOrAbsolute));
}
