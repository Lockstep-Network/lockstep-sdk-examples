using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;

namespace LockstepExamples
{
    public static class ConnectorExample
    {
        public static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync(async o =>
                {
                    // First, connect to the SFTP site and download the file
                    var downloader = new FileDownloader(o.Hostname, o.Username, o.Password, o.PortNumber, o.KeyFile);
                    var incomingFiles = downloader.RetrieveFiles();
                    try
                    {
                        // We've collected a list of files. Let's parse them into a collection of objects
                        var invoiceXml = await FileParser.ParseInvoices(incomingFiles);
                        var invoices = ModelConverter.ConvertInvoices(invoiceXml);
                        
                        // Save invoices to CSV files
                        var filenames = new List<string> { await ModelConverter.SaveBatchToCsv(invoices, "invoices.csv") };

                        // Let's convert them to the Lockstep SFTP format
                        var zipArchiveName = $"{DateTime.Today:yyyy-MM-dd}_batch.zip";
                        await BatchSubmitter.CreateZipFile(zipArchiveName, filenames);

                        // Upload the collection of data to the Lockstep API
                        var sync = await BatchSubmitter.UploadToLockstep(o.ServerUrl, o.ApiKey, zipArchiveName);

                        // Retrieve results
                        await BatchSubmitter.WaitForSyncComplete(o.ServerUrl, o.ApiKey, sync.SyncRequestId, 5);
                    }
                    finally
                    {
                        foreach (var f in incomingFiles.Where(f => File.Exists(f)))
                        {
                            File.Delete(f);
                        }
                    }

                });
        }
    }
}