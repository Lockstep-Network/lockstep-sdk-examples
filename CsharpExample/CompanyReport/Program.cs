using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LockstepSDK;
using CsvHelper;

namespace LockstepExamples // Note: actual namespace depends on the project name.
{
    public class CompanyReport
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
            var count = 0;
            
            List<Entry> entries = new();
            
            while (true)
            {
                var companies = await client.Companies.QueryCompanies(
                    null, 
                    null, 
                    null, 
                    100, 
                    pageNumber
                );

                if (!companies.Success || companies.Value.Records.Length == 0)
                {
                    break;
                }

                foreach (var company in companies.Value.Records)
                {
                    Console.WriteLine($"Company: {company.CompanyName}");
                    Console.WriteLine($"Phone: {company.PhoneNumber}");
                    Console.WriteLine($"ApEmail: {company.ApEmailAddress}");
                    Console.WriteLine($"ArEmail: {company.ArEmailAddress}");
                    Console.WriteLine();

                    entries.Add((new Entry
                    {
                        Company = company.CompanyName, 
                        Phone = company.PhoneNumber,
                        ApEmail = company.ApEmailAddress,
                        ArEmail = company.ArEmailAddress
                    }));
                }
                
                var currentDirectory = Directory.GetCurrentDirectory();
                const string fileName = "Results.csv";
                var filePath = $"{currentDirectory}\\..\\..\\..\\{fileName}";
                
                try
                {
                    Console.WriteLine($"Writing contents to CSV file \"{fileName}\"...");
                    await using (var writer = new StreamWriter($"{filePath}"))
                    await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        await csv.WriteRecordsAsync(entries);
                    }
                    Console.WriteLine($"Successfully created file: {filePath}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while creating file: {e.Message}");
                }

                pageNumber++;
            }
        }
    }
    
    public class Entry
    {
        public string? Company { get; set; }
        public string? Phone { get; set; }
        public string? ApEmail { get; set; }
        public string? ArEmail { get; set; }
    }
}