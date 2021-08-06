using System.Linq;
using System.Collections.Generic;

namespace PricingApp
{
    public class Model
    {
        public record PerMinute(int Minute, double Vcpu, double GibMemory, int Requests, double Price);
        public record Price(double PerMonth, List<PerMinute> PerMinute);
        public Price Steady(double Vcpu, double GibMemory, double requestsPerSecond)
        {
            var request = new RequestProfile(Vcpu / requestsPerSecond, GibMemory / requestsPerSecond);
            var calculator = new Calculator();
            double price = calculator.PerSecond(request) * 60;
            
            List<PerMinute> perMinute = new List<PerMinute>();
            
            perMinute.Add(new PerMinute(1, 
                request.VcpuPerRequest, 
                request.GiBMemoryPerRequest, 
                (int)(requestsPerSecond * 60),
                price));
            
            for (int i = 1; i < (Calculator.SecondsPerMonth/60); i++)
            {
                perMinute.Add(perMinute[i-1] with { 
                    Minute = i,
                    Requests = (int)(requestsPerSecond * i)
                });
            }
            
            var result = new Price(perMinute.Sum(p => p.Price), perMinute);
            return result;
        }
    }
}