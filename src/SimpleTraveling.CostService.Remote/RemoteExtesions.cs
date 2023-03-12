using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SimpleTraveling.Remote;

namespace SimpleTraveling.CostService.Remote;

public static class RemoteExtesions
{
    public static RemoteBuilder AddBillsRemote(this RemoteBuilder builder) =>
        builder.Add<BillsRemote>((provider, options) =>
            options.BaseAddress = new(provider.GetRequiredService<IConfiguration>().GetConnectionString("costservice")!, UriKind.RelativeOrAbsolute));

    public static RemoteBuilder AddDiscountsRemote(this RemoteBuilder builder) =>
        builder.Add<DiscountsRemote>((provider, options) =>
            options.BaseAddress = new(provider.GetRequiredService<IConfiguration>().GetConnectionString("costservice")!, UriKind.RelativeOrAbsolute));
}
