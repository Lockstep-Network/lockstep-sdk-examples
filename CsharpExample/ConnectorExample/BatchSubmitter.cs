﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading.Tasks;
using LockstepSDK;

namespace LockstepExamples
{
    public static class BatchSubmitter
    {
        /// <summary>
        /// Creates a zip file containing csv files
        /// </summary>
        /// <param name="zipArchiveName"></param>
        /// <param name="csvFileNames"></param>
        public static async Task CreateZipFile(string zipArchiveName, IEnumerable<string> csvFileNames)
        {
            using (var zipArchive = ZipFile.Open(zipArchiveName, ZipArchiveMode.Create))
            {
                foreach (var csvName in csvFileNames)
                {
                    if (!File.Exists(csvName)) continue;
                    var zippedFile = zipArchive.CreateEntry(csvName);

                    using (var entryStream = zippedFile.Open())
                    {
                        using (var fs = File.Open(csvName, FileMode.Open))
                        {
                            await fs.CopyToAsync(entryStream);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Upload this zip file to the Upload Zip File API
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="zipArchiveName"></param>
        /// <param name="serverUrl"></param>
        public static async Task<SyncRequestModel> UploadToLockstep(string serverUrl, string apiKey,
            string zipArchiveName)
        {
            var client = LockstepApi
                .WithCustomEnvironment(serverUrl)
                .WithApiKey(apiKey);
            var result = await client.Sync.UploadSyncFile(zipArchiveName);
            if (result.Success && result.Value != null)
            {
                return result.Value;
            }
            throw new Exception($"Failed to upload zip file: {result.Error?.Content}");
        }

        /// <summary>
        /// Check the Sync process to see when it completes
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <param name="apiKey"></param>
        /// <param name="retrieveGuid">The unique ID number of the Sync task to retrieve</param>
        /// <param name="maxMin">max number of minutes the API retries before timing out</param>
        public static async Task WaitForSyncComplete(string serverUrl, string apiKey,
            Guid retrieveGuid, int maxMin)
        {
            var client = LockstepApi.WithCustomEnvironment(serverUrl)
                .WithApiKey(apiKey);

            var maxTime = DateTime.UtcNow.AddMinutes(maxMin);
            while (DateTime.UtcNow < maxTime)
            {
                var response = await client.Sync.RetrieveSync(retrieveGuid, "Details");
                Console.WriteLine($"{DateTime.UtcNow} - Sync status: {response.Value?.StatusCode}");

                switch (response.Value?.StatusCode)
                {
                    // Check if the sync process succeeded
                    case "Success":
                        Console.WriteLine($"{DateTime.UtcNow} - All data is now loaded into Lockstep.");
                        return;
                    case "Failed":
                        Console.WriteLine($"{DateTime.UtcNow} - Batch could not be completed.");
                        Console.WriteLine();
                        Console.WriteLine("Detailed logs: ");
                        Console.WriteLine(JsonSerializer.Serialize(response.Value?.Details));
                        break;
                }

                // Wait two seconds to check for progress again
                await Task.Delay(2000);
            }

            // We couldn't get answers in the time allotted
            Console.WriteLine($"{DateTime.UtcNow} - Waited for the maximum of {maxMin}.");
        }
    }
}