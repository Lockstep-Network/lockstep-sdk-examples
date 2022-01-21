using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LockstepSDK;

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
                }

                pageNumber++;
            }
        }
    }
}