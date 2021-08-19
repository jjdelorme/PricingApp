using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using PricingApp;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var model = new Model();

app.MapGet("/", () => File.ReadAllText("index.html"));
app.MapGet("/steady", () => model.Steady(1.0, 3.0, 300.0, 100.0));
app.MapGet("/bursty", () => model.Bursty(1.0, 3.0, 300.0, 100.0));

app.Run();

// var bursty = model.Bursty(1.0, 3.0, 300);
// var steady = model.Steady(1.0, 3.0, 300);

// // var burstyJson = System.Text.Json.JsonSerializer.Serialize(bursty);
// // var burstyJson = System.Text.Json.JsonSerializer.Serialize(bursty);


// Console.WriteLine($"Bursty: {bursty.Requests}, {bursty.PerMonth}");
// Console.WriteLine($"Steady: {steady.Requests}, {steady.PerMonth}");

// foreach (var minute in bursty.PerMinute)
// {
//     Console.WriteLine($"{minute.Minute}, {minute.Price}");
// }
