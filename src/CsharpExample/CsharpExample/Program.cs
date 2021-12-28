using System;
using System.Collections.Generic;
using System.Linq;
using LockstepSDK;
using RestSharp.Serialization.Json;

namespace LockstepExamples // Note: actual namespace depends on the project name.
{
    public class CSharpExample
    {
        public static async Task Main(string[] args)
        {
            var serializer = new JsonSerializer();
            var client = LockstepApi
                .withEnvironment(LockstepEnv.SBX);
            var apiKey = Environment.GetEnvironmentVariable("LOCKSTEPAPI");
            if (apiKey != null)
            {
                client.withApiKey(apiKey);
            }
            var result = await client.Status.Ping();
            
            Console.WriteLine("Ping result: " + serializer.Serialize(result));
        }
    }
}