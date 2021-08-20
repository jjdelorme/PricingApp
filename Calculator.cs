using System;

namespace PricingApp
{    
    public class Calculator
    {
        private double _vcpu, _gibMemory, _concurrency;
        private int _cumulativeRequests;
        private double _cumulativePrice;
        private CloudRunPricing _pricing;

        private double _computePrice;

        public double CumulativePrice { get { return _cumulativePrice;} }
        public int CumulativeRequests { get { return _cumulativeRequests;} }

        public Calculator(double vcpu, double gibMemory, double concurrency)
        {
            _vcpu = vcpu;
            _gibMemory = gibMemory;
            _concurrency = concurrency;
            _cumulativeRequests = 0;
            _cumulativePrice = 0;

            _pricing = CloudRunPricing.GetPricing();
            
            _computePrice = 
                (_vcpu * _pricing.Vcpu.PerSecond) +
                (_gibMemory * _pricing.Memory.GiBPerSecond);            
        }        
       
        public void PerSecond(int requests, out int instances, out double price)
        {
            double requestsPrice = requests * _pricing.Requests.PerRequest;

            instances = (int)Math.Ceiling(requests/_concurrency);

            price = (instances * _computePrice) + requestsPrice;

            _cumulativePrice += price;
            _cumulativeRequests += requests;
        }

        public void PerMinute(int requests, out int instances, out double price)
        {
            if (requests <= 0)
            {
                price = 0;
                instances = 0;
                return;
            }
            
            double requestsPrice = requests * _pricing.Requests.PerRequest;

            instances = (int)Math.Ceiling((requests/60.0)/_concurrency);

            price = (instances * _computePrice) + requestsPrice;
            
            _cumulativePrice += price;
            _cumulativeRequests += requests;            
        }

        public void PerMinuteIdle(double vcpu, double gib, double requests, double concurrency, 
        out int instances, out double price)
        {
            instances = 0;
            price = 0;
        }

        public record Price(int Instances, double TotalPrice);
    }
}