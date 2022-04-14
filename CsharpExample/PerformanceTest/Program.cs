using System.Text.Json;
using LockstepSDK;

namespace LockstepExamples 
{
    public static class PerformanceTest
    {
        public static async Task Main(string[] args)
        {
            var client = LockstepApi.WithEnvironment("sbx")
                .WithApiKey(Environment.GetEnvironmentVariable("LOCKSTEPAPI_SBX"));

            // Test first API call
            var result = await client.Status.Ping();
            if (!result.Success || result.Value.LoggedIn != true)
            {
                Console.WriteLine("Your API key is not valid.");
                Console.WriteLine("Please set the environment variable LOCKSTEPAPI_SBX and try again.");
                return;
            }

            // Basic diagnostics
            Console.WriteLine($"Ping result: {result.Value.UserName} ({result.Value.UserStatus})");
            Console.WriteLine($"Server status: {result.Value.Environment} {result.Value.Version}");
            Console.WriteLine();
            
            // Run a series of a thousand queries for invoices
            var performance = new List<Tuple<long, int>>();
            while (performance.Count < 200)
            {
                var pageNumber = 0;

                while (true)
                {
                    var invoices = await client.Invoices.QueryInvoices(
                        null,
                        null,
                        null,
                        100,
                        pageNumber
                    );
                    performance.Add( new Tuple<long, int>( invoices.TotalRoundtrip, invoices.ServerDuration));

                    if (!invoices.Success || invoices.Value?.Records?.Length == 0) break;
                    pageNumber++;
                }
            }
            
            // Sum up the performance reports
            var averageRoundtrip = (from t in performance select t.Item1).Average();
            var averageServerDuration = (from t in performance select t.Item2).Average();
            Console.WriteLine($"Number of calls: {performance.Count}");
            Console.WriteLine($"Average roundtrip: {averageRoundtrip}");
            Console.WriteLine($"Average server: {averageServerDuration}");
            Console.WriteLine("Done");
        }
    }
}