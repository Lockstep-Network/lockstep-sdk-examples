using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using CSVFile;
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
        
        public static async Task<string> SaveBatchToCsv(IEnumerable<InvoiceSyncModel> list, string filename)
        {
            var filePath = Path.Combine(Path.GetTempPath(), filename);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            await File.WriteAllTextAsync(filePath, CSV.Serialize(list));
            return filePath;
        }
    }
}