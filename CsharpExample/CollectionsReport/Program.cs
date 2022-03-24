using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CSVFile;
using LockstepSDK;

// I would like to see a sample program that shows how to query for invoices with an outstanding balance older than a certain age,
// then fetch the primary contact person for that invoice, and export the list to CSV

namespace LockstepExamples // Note: actual namespace depends on the project name.
{
    public static class CollectionsReport
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
            
            List<Entry> entries = new();
            
            while (true)
            {
                var invoices = await client.Invoices.QueryInvoices(
                    "invoiceDate < 2021-12-01 AND outstandingBalanceAmount > 0", 
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
                    entries.Add((new Entry
                    {
                        InvoiceId = invoice.InvoiceId,
                        InvoiceDate = invoice.InvoiceDate,
                        OutstandingBalance = invoice.OutstandingBalanceAmount,
                        PrimaryContact = invoice.CustomerPrimaryContact?.ContactName
                    }));
                }
                
                var filePath = Path.GetTempFileName();
                try
                {
                    await File.WriteAllTextAsync(filePath, CSV.Serialize(entries));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while creating file: {e.Message}");
                }

                pageNumber++;
            }
            
            Console.WriteLine("Please check project directory for results.");
        }
    }
    
    public class Entry
    {
        public Guid? InvoiceId { get; set; }
        public string? InvoiceDate { get; set; }
        public double? OutstandingBalance { get; set; }
        public string? PrimaryContact { get; set; }
    }
}