using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Text;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;

namespace WpfAppFlowlayout
{
    public static class FlowToXps
    {
        /// <summary>
        /// source: https://stackoverflow.com/a/3110276/534812
        /// </summary>
        /// <param name="path"></param>
        /// <param name="document"></param>
        public static void SaveAsXps(string path, FlowDocument document)
        {
            using (Package package = Package.Open(path, FileMode.Create))
            {
                using (var xpsDoc = new XpsDocument(package, CompressionOption.NotCompressed))
                {
                    var xpsSm = new XpsSerializationManager(new XpsPackagingPolicy(xpsDoc), false);
                    DocumentPaginator dp = ((IDocumentPaginatorSource)document).DocumentPaginator;
                    xpsSm.SaveAsXaml(dp);
                }
            }
        }
    }
}
