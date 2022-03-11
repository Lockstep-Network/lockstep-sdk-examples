using System;
using System.Text.Json;
using System.Threading.Tasks;
using LockstepSDK;

namespace LockstepExamples // Note: actual namespace depends on the project name.
{
    public static class CSharpExample
    {
        public static async Task Main(string[] args)
        {
            var client = LockstepApi.WithEnvironment(LockstepEnv.SBX)
                .WithApiKey(Environment.GetEnvironmentVariable("LOCKSTEPAPI_SBX"));

            // Test first API call
            var result = await client.Status.Ping();
            if (!result.Success || !result.Value.LoggedIn)
            {
                Console.WriteLine("Your API key is not valid.");
                Console.WriteLine("Please set the environment variable LOCKSTEPAPI_SBX and try again.");
                return;
            }

            // Basic diagnostics
            Console.WriteLine($"Ping result: {result.Value.UserName} ({result.Value.UserStatus})");
            Console.WriteLine($"Server status: {result.Value.Environment} {result.Value.Version}");
            Console.WriteLine();

            var pageNumber = 0;
            var count = 0;
            
            while (true)
            {
                // Single API call to fetch invoices and company info.
                // Pass "Customer" instead of "Company" into the "include" parameter
                // because "Company" was returning the main CompanyName "Lockstep Demo Data"
                // instead of each individual invoice's company name.
                var invoices = await client.Invoices.QueryInvoices(
                    "invoiceDate > 2021-12-01", 
                    "Customer", 
                    "invoiceDate asc", 
                    100, 
                    pageNumber
                );

                if (invoices.Value?.Records == null)
                {
                    break;
                }
                
                foreach (var invoice in invoices.Value.Records)
                {
                    Console.WriteLine($"Invoice {count++}: {invoice.InvoiceId}");
                    Console.WriteLine($"Company Name: {invoice.Customer?.CompanyName}");
                    Console.WriteLine($"Outstanding Balance: {invoice.OutstandingBalanceAmount:C}");
                    Console.WriteLine(); // Space for readability.
                }

                pageNumber++;
            }
        }
    }
}