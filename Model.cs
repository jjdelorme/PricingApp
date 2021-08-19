using System;
using System.Linq;
using System.Collections.Generic;

namespace PricingApp
{
    public class Model
    {
        public record PerMinute(int Minute, double Vcpu, double GibMemory, double Requests, double Price);
        public record Price(double PerMonth, double Requests, List<PerMinute> PerMinute);
        public Price Steady(double Vcpu, double GibMemory, double requestsPerSecond, double concurrency = 100)
        {
            var request = new RequestProfile(Vcpu / requestsPerSecond, GibMemory / requestsPerSecond);
            var calculator = new Calculator();
            double price = calculator.PerSecond(request) * 60;
            
            List<PerMinute> perMinute = new List<PerMinute>();
            
            perMinute.Add(new PerMinute(1, 
                request.VcpuPerRequest, 
                request.GiBMemoryPerRequest, 
                (requestsPerSecond * 60),
                price));
            
            for (int i = 1; i < (Calculator.SecondsPerMonth/60); i++)
            {
                perMinute.Add(perMinute[i-1] with { 
                    Minute = i,
                    Requests = (requestsPerSecond * 60)
                });
            }
            
            var result = new Price(
                perMinute.Sum(p => p.Price), 
                perMinute.Sum(r => r.Requests),
                perMinute);
            return result;
        }

        public Price Bursty(double Vcpu, double GibMemory, double averageRequestsPerSecond, double concurrency = 100.0)
        {
            // Pod is either running, idle or not running.  Concurrency determines the # of requests a single pod
            // can handle at once.
            // Vcpu and GiBMemory specify the pod size

            const int DailyMinutes = (int)(24 * 60);
            const double k = 0.05;
            List<PerMinute> perMinute = new List<PerMinute>();
            Calculator calculator = new Calculator();
            double averageRequestsPerMinute = averageRequestsPerSecond * 60.0;
            double cumulativePrice = 0.0;

            for (int minute = 1; minute < DailyMinutes; minute++)
            {
                double requests = (averageRequestsPerMinute * Math.Sin(minute * k)) + 
                    averageRequestsPerMinute;
                                
                if (requests > 0)
                {
                    double price = calculator.PerMinute(Vcpu, GibMemory, requests, concurrency);
                    cumulativePrice += price;

                    perMinute.Add(new PerMinute(minute, 
                        Vcpu, 
                        GibMemory, 
                        requests,
                        cumulativePrice));
                }
                else
                {
                    // Calculate idle price?
                    perMinute.Add(
                        new PerMinute(minute, 0, 0, 0, 0)
                    );
                }
            }
            
            var result = new Price(
                cumulativePrice, 
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