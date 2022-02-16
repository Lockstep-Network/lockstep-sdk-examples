using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using CsvHelper;
using LockstepSDK;

namespace LockstepExamples
{
    public static class ModelConverter
    {
        public static IEnumerable<InvoiceSyncModel> ConvertInvoices(List<XmlDocument> invoices)
        {
            // Here you will convert your XML documents to Lockstep InvoiceSyncModel objects.
            return System.Array.Empty<InvoiceSyncModel>();
        }
        
        public static string SaveBatchToCsv(IEnumerable<InvoiceSyncModel> list, string filename)
        {
            var filePath = Path.Combine(Path.GetTempPath(), filename);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            using var writer = new StreamWriter(filePath);
            using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csvWriter.WriteRecords(list);
            return filePath;
        }
    }
}