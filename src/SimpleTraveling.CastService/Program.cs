﻿using SimpleTraveling.Abstractions.Converter;
using SimpleTraveling.CostService.Binders;
using SimpleTraveling.CostService.Data;
using SimpleTraveling.CostService.Services;
using SimpleTraveling.DriverService.Remote;
using SimpleTraveling.Remote;
using SimpleTraveling.TravelService.Remote;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRemoteService()
    .AddPassengersRemote()
    .AddTravelsRemote()
    .AddDriversRemote();

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.AddSingleton<DiscountService>();
builder.Services.AddSingleton<AmountService>();

builder.Services.AddSingleton<DataContext>();
builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration.GetConnectionString("redis"));

builder.Services.AddControllers(options =>
    options.ModelBinderProviders.Add(new ObjectIdModelBinderProvider()))
    .AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new ObjectIdConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

await app.RunAsync().ConfigureAwait(false);

namespace SimpleTraveling.CostService
{
    public partial class Program { }
}
