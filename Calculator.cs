using System;

namespace PricingApp
{    
    public class Calculator
    {
        public record Price(int Instances, double TotalPrice);
        private CloudRunPricing _pricing = CloudRunPricing.GetPricing();

        //public const double SecondsPerMonth = 3600 * 730;
        
        public double PerSecond(RequestProfile request)
        {
            double price = request.VcpuPerRequest * _pricing.Vcpu.PerSecond +
                request.GiBMemoryPerRequest * _pricing.Memory.GiBPerSecond + 
                _pricing.Requests.PerRequest;
            
            return price;
        }

        public void PerMinute(double vcpu, double gib, double requests, double concurrency, 
            out int instances, out double price)
        {
            double requestsPrice = requests * _pricing.Requests.PerRequest;
            double computePrice = 
                (vcpu * _pricing.Vcpu.PerSecond * 60.0) +
                (gib * _pricing.Memory.GiBPerSecond * 60.0);

            instances = (int)Math.Ceiling((requests/60.0)/concurrency);

            price = (instances * computePrice) + requestsPrice;
        }

        public void PerMinuteIdle(double vcpu, double gib, double requests, double concurrency, 
            out int instances, out double price)
            {
                instances = 0;
                price = 0;
            }
    }
}