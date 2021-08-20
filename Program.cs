using System;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using PricingApp;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var model = new Model();

app.MapGet("/", () => File.ReadAllText("index.html"));
app.MapGet("/steady", () => model.Steady(1.0, 3.0, 300.0, 100.0));

app.MapGet("/bursty", async context => {
    double vcpu = 1.0, gib = 3.0, rps = 300.0, concurrency = 100.0;
    double.TryParse(context.Request.Query["vcpu"].ToString(), out vcpu);
    double.TryParse(context.Request.Query["gib"].ToString(), out gib);
    double.TryParse(context.Request.Query["rps"].ToString(), out rps);
    double.TryParse(context.Request.Query["c"].ToString(), out concurrency);
    
    var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    string json = JsonSerializer.Serialize<Model.Price>(
        model.Bursty(vcpu, gib, rps, concurrency), options);
    
    await context.Response.WriteAsync(json);
});

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
