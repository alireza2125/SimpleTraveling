using Microsoft.EntityFrameworkCore;

using SimpleTraveling.CostService.Services;
using SimpleTraveling.Remote;
using SimpleTraveling.TravelService.Data;
using SimpleTraveling.TravelService.Remote;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRemoteService()
    .AddBillsRemote()
    .AddDiscountsRemote();

builder.Services.AddSingleton<DistanceService>();

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("mssql")));
builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration.GetConnectionString("redis"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await using (var asyncScope = app.Services.CreateAsyncScope())
{
    await asyncScope.ServiceProvider.GetRequiredService<DataContext>().Database.MigrateAsync().ConfigureAwait(false);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

await app.RunAsync().ConfigureAwait(false);

namespace SimpleTraveling.TravelService
{
    public partial class Program { }
}

