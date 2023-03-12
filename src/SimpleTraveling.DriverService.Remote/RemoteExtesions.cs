using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SimpleTraveling.Remote;

namespace SimpleTraveling.DriverService.Remote;

public static class RemoteExtesions
{
    public static RemoteBuilder AddDriversRemote(this RemoteBuilder builder) =>
        builder.Add<DriverRemote>((provider, options) =>
            options.BaseAddress = new(provider.GetRequiredService<IConfiguration>().GetConnectionString("driverservice")!, UriKind.RelativeOrAbsolute));
}
