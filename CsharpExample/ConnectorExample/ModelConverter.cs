using System.Collections.Generic;
using System.Xml;
using LockstepSDK;

namespace LockstepExamples
{
    public class ModelConverter
    {
        public static IEnumerable<InvoiceSyncModel> ConvertInvoices(List<XmlDocument> invoices)
        {
            return new InvoiceSyncModel[] { };
        }
    }
}