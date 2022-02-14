using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommandLine;

namespace LockstepExamples
{
    public class ConnectorExample
    {
        public static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync<Options>(async o =>
                {
                    // First, connect to the SFTP site and download the file
                    var downloader = new FileDownloader(o.Hostname, o.Username, o.Password, o.PortNumber, o.KeyFile);
                    var incomingFiles = downloader.RetrieveFiles();
                    try
                    {
                        // We've collected a list of files. Let's parse them into a collection of objects
                        var invoices = await FileParser.ParseInvoices(incomingFiles);
                        List<string> filenames = new List<string>();
                        var files = ModelConverter.ConvertInvoices(invoices);

                        // Let's convert them to the Lockstep SFTP format
                        var zipArchiveName = $"{DateTime.Today.ToString("yyyy-MM-dd")}_batch.zip";
                        await BatchSubmitter.CreateZipFile(zipArchiveName, filenames);

                        // Upload the collection of data to the Lockstep API
                        var sync = await BatchSubmitter.UploadToLockstep(o.ServerUrl, o.ApiKey, zipArchiveName);

                        // Retrieve results
                        await BatchSubmitter.WaitForSyncComplete(o.ServerUrl, o.ApiKey, sync.SyncRequestId, 5);
                    }
                    finally
                    {
                        foreach (var f in incomingFiles)
                        {
                            if (File.Exists(f))
                            {
                                File.Delete(f);
                            }
                        }
                    }

                });
        }
    }
}