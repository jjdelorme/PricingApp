
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
    }
}