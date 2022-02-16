using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using LockstepSDK;
using CsvHelper;

// I would like to see a sample program that shows how to query for invoices with an outstanding balance older than a certain age,
// then fetch the primary contact person for that invoice, and export the list to CSV

namespace LockstepExamples // Note: actual namespace depends on the project name.
{
    public static class CollectionsReport
    {
        public static async Task Main(string[] args)
        {
            var client = LockstepApi.WithEnvironment(LockstepEnv.SBX);
            var apiKey = Environment.GetEnvironmentVariable("LOCKSTEPAPI_SBX");
            
            if (apiKey != null)
            {
                client.WithApiKey(apiKey);
            }
            
            var result = await client.Status.Ping();
            Console.WriteLine("Ping result: " + JsonSerializer.Serialize(result));
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
                
                var currentDirectory = Directory.GetCurrentDirectory();
                const string fileName = "Results.csv";
                var filePath = $"{currentDirectory}\\..\\..\\..\\{fileName}";

                try
                {
                    await using (var writer = new StreamWriter($"{filePath}"))
                    await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        await csv.WriteRecordsAsync(entries);
                    }
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
        public DateTime? InvoiceDate { get; set; }
        public double? OutstandingBalance { get; set; }
        public string? PrimaryContact { get; set; }
    }
}