using System;
using System.Linq;
using System.Collections.Generic;

namespace PricingApp
{
    public class Model
    {
        public record PerMinute(int Minute, int Instances, double Requests, double Price);
        public record Price(double PerMonth, double Requests, List<PerMinute> PerMinute);
        public Price Steady(double Vcpu, double GibMemory, double requestsPerSecond, double concurrency = 100)
        {
            // var request = new RequestProfile(Vcpu / requestsPerSecond, GibMemory / requestsPerSecond);
            // var calculator = new Calculator();
            // double price = calculator.PerSecond(request) * 60;
            
            // List<PerMinute> perMinute = new List<PerMinute>();
            
            // perMinute.Add(new PerMinute(1, 
            //     request.VcpuPerRequest, 
            //     request.GiBMemoryPerRequest, 
            //     (requestsPerSecond * 60),
            //     price));
            
            // for (int i = 1; i < (Calculator.SecondsPerMonth/60); i++)
            // {
            //     perMinute.Add(perMinute[i-1] with { 
            //         Minute = i,
            //         Requests = (requestsPerSecond * 60)
            //     });
            // }
            
            // var result = new Price(
            //     perMinute.Sum(p => p.Price), 
            //     perMinute.Sum(r => r.Requests),
            //     perMinute);
            // return result;
            return default(Price);
        }

        public Price Bursty(double Vcpu, double GibMemory, double averageRequestsPerSecond, double concurrency = 100.0)
        {
            const int DailyMinutes = 24 * 60;
            const double k = 0.015;
            List<PerMinute> perMinute = new List<PerMinute>();
            Calculator calculator = new Calculator(Vcpu, GibMemory, concurrency);

            for (int minute = 1; minute < DailyMinutes+1; minute++)
            {
                int instances;
                double price;

                // Model requests.
                int requests = 60 * (int)(Math.Ceiling((averageRequestsPerSecond * 
                    Math.Sin(minute * k)) + averageRequestsPerSecond));
                                
                calculator.PerMinute(requests, out instances, out price);

                perMinute.Add(new PerMinute(minute, 
                    instances,
                    requests,
                    calculator.CumulativePrice));
            }
            
            var result = new Price(
                calculator.CumulativePrice * 30, 
                calculator.CumulativeRequests * 30,
                perMinute);

            return result;            
        }

        // Assumes exponential growth
        public Price Growth(double Vcpu, double GibMemory, double requestsPerSecond)
        {
            return null;
        }
    }
}