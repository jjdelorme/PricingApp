using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using PricingApp;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var model = new Model();

app.MapGet("/", () => File.ReadAllText("index.html"));
app.MapGet("/steady", () => model.Steady(1.0, 3.0, 1));
app.MapGet("/bursty", () => model.Bursty(1.0, 3.0, 1));

app.Run();