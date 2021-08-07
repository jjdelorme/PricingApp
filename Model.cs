using System;
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

        public Price Bursty(double Vcpu, double GibMemory, double averageRequestsPerSecond)
        {
            // average
            // peak
            // trough
            /*
                var rpm = 3 * 60;
                var k = .05;
                const r = [...Array(1000).keys()];
                r.forEach(i => data.addRow([i, (rpm * Math.sin(i*k))+rpm, 0]));
            */

            // Calculate per Minute.
            const int MonthlyMinutes = (int)(730 * 60);
            const double k = 0.05;
            List<PerMinute> perMinute = new List<PerMinute>();
            int LastRequests(int minute)
            {
                if (minute > 1)
                    return perMinute[minute-2].Requests;
                else
                    return 0;
            }

            for (int minute = 1; minute < MonthlyMinutes; minute++)
            {
                double averageRequestsPerMinute = averageRequestsPerSecond * 60.0;
                int requestsPerMinute = (int)((averageRequestsPerMinute * Math.Sin(minute * k)) + 
                    averageRequestsPerMinute);
                double requestsPerSecond = requestsPerMinute / 60.0;
                
                if (requestsPerSecond > 0)
                {
                    var request = new RequestProfile(Vcpu / requestsPerSecond, GibMemory / requestsPerSecond);
                    var calculator = new Calculator();
                    double price = calculator.PerSecond(request) * 60.0;

                    perMinute.Add(new PerMinute(minute, 
                        request.VcpuPerRequest, 
                        request.GiBMemoryPerRequest, 
                        LastRequests(minute) + requestsPerMinute,
                        price));
                }
                else
                {
                    perMinute.Add(
                        new PerMinute(minute,0, 0, LastRequests(minute), 0)
                    );
                }
            }
            
            var result = new Price(perMinute.Sum(p => p.Price), perMinute);
            return result;
        }

        // Assumes exponential growth
        public Price Growth(double Vcpu, double GibMemory, double requestsPerSecond)
        {
            return null;
        }
    }
}