using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CSVFile;
using LockstepSDK;

namespace LockstepExamples // Note: actual namespace depends on the project name.
{
    public static class CompanyReport
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
                var companies = await client.Companies.QueryCompanies(
                    null, 
                    null, 
                    null, 
                    100, 
                    pageNumber
                );

                if (companies.Value?.Records == null)
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

                var filePath = Path.GetTempFileName();
                
                try
                {
                    Console.WriteLine($"Writing contents to CSV file \"{filePath}\"...");
                    await File.WriteAllTextAsync(filePath, CSV.Serialize(entries));
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