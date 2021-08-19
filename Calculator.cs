using System;

namespace PricingApp
{    
    public class Calculator
    {
        private CloudRunPricing _pricing = CloudRunPricing.GetPricing();

        public const double SecondsPerMonth = 3600 * 730;
        
        public double MonthlyCost(RequestProfile request, double RequestsPerSecond)
        {
            return PerSecond(request) * RequestsPerSecond * SecondsPerMonth;
        }

        public double PerSecond(RequestProfile request)
        {
            double price = request.VcpuPerRequest * _pricing.Vcpu.PerSecond +
                request.GiBMemoryPerRequest * _pricing.Memory.GiBPerSecond + 
                _pricing.Requests.PerRequest;
            
            return price;
        }

        public double PerMinute(double vcpu, double gib, double requests, double concurrency)
        {
            double requestsPrice = requests * _pricing.Requests.PerRequest;
            double computePrice = 
                (vcpu * _pricing.Vcpu.PerSecond * 60) +
                (gib * _pricing.Memory.GiBPerSecond * 60);

            int instances = (int)Math.Ceiling(requests/concurrency);

            double price = (instances * computePrice) + requestsPrice;

            return price;
        }
    }
}