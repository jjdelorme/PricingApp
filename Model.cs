using System;
using System.Linq;
using System.Collections.Generic;

namespace PricingApp
{
    public class Model
    {
        public record PerMinute(int Minute, double Vcpu, double GibMemory, int Requests, double Price);
        public record Price(double PerMonth, int Requests, List<PerMinute> PerMinute);
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
                    Requests = (int)(requestsPerSecond * 60)
                });
            }
            
            var result = new Price(
                perMinute.Sum(p => p.Price), 
                perMinute.Sum(r => r.Requests),
                perMinute);
            return result;
        }

        public Price Bursty(double Vcpu, double GibMemory, double averageRequestsPerSecond)
        {
            const int MonthlyMinutes = (int)(730 * 60);
            const double k = 0.05;
            List<PerMinute> perMinute = new List<PerMinute>();

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
                        requestsPerMinute,
                        price));
                }
                else
                {
                    perMinute.Add(
                        new PerMinute(minute, 0, 0, 0, 0)
                    );
                }
            }
            
            var result = new Price(
                perMinute.Sum(p => p.Price), 
                perMinute.Sum(r => r.Requests),
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