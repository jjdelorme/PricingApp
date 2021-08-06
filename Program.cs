using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

CloudRunPricing pricing = CloudRunPricing.GetPricing();

app.MapGet("/", () => $"Price per vCpu Second = {pricing.Vcpu.CudPerSecond:F8}");

app.Run();