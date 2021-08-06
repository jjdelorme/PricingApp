using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public record CloudRunPricing(Requests Requests, Memory Memory, Vcpu Vcpu)
{
    public static CloudRunPricing GetPricing()
    {
        string json = File.ReadAllText("Pricing.json");
        CloudRunPricing pricing = 
            JsonSerializer.Deserialize<CloudRunPricing>(json);
        return pricing;
    }
}

/*
"Requests": {
    "Free": 2000000,
    "PerRequest": 0.0000004,
    "CudPerRequest": 0.000000332
},
*/
public record Requests(int Free, double PerRequest, double CudPerRequest);

/*
"Memory": {
    "Free": 360000,
    "GiBPerSecond": 0.00000250,
    "CudGiBPerSecond": 0.000002075,
    "IdlePerSecond": 0.00000250
},
*/
public record Memory(int Free, double GiBPerSecond, double CudGiBPerSecond, double IdlePerSecond);
/*
"vCpu": {
    "FreeSeconds": 180000,
    "PerSecond": 0.00002400,
    "CudPerSecond": 0.00001992,
    "IdlePerSecond": 0.00000250
}
*/
public record Vcpu(int FreeSeconds, double PerSecond, double CudPerSecond, double IdlePerSecond);