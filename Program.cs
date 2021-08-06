using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using PricingApp;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var model = new Model();

app.MapGet("/", () => model.Steady(1.0, 3.0, 1));

app.Run();