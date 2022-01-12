using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LockstepSDK;

namespace LockstepExamples // Note: actual namespace depends on the project name.
{
    public class CSharpExample
    {
        public static async Task Main(string[] args)
        {
            var client = LockstepApi
                .WithEnvironment(LockstepEnv.SBX);
            var apiKey = Environment.GetEnvironmentVariable("LOCKSTEPAPI_SBX");
            if (apiKey != null)
            {
                client.WithApiKey(apiKey);
            }
            var result = await client.Status.Ping();
            
            Console.WriteLine("Ping result: " + JsonSerializer.Serialize(result));
            
            var pageNumber = 0;
            var count = 0;
            
            while (true)
            {
                var invoices = await client.Invoices.QueryInvoices("invoiceDate > 2021-12-01", null, "invoiceDate asc", 100, pageNumber);
                if (!invoices.Success || invoices.Value.Records.Length == 0)
                {
                    break;
                }
                foreach (var invoice in invoices.Value.Records)
                {
                    Console.WriteLine($"Invoice {count++}: {invoice.InvoiceId}");
                }

                pageNumber++;
            }
            
            
        }
    }
}