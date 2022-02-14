using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
                var contents = await File.ReadAllTextAsync(file);
                doc.LoadXml(contents);
                list.Add(doc);
            }

            return list;
        }
    }
}