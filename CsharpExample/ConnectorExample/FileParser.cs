using System.Xml;

namespace LockstepExamples
{
    public class FileParser
    {
        /// <summary>
        /// Loads in all documents as XML files
        /// </summary>
        /// <param name="incomingFiles"></param>
        /// <returns></returns>
        public static async Task<List<XmlDocument>> ParseInvoices(List<string> incomingFiles)
        {
            var list = new List<XmlDocument>();
            foreach (var file in incomingFiles)
            {
                var doc = new XmlDocument();
                doc.Load(file);
                list.Add(doc);
            }

            return list;
        }
    }
}