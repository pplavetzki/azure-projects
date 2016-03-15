using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PareidoliaFileViewer.Models
{
    public class SASResponse
    {
        public string Id { get; set; }
        public string StorageAccountName { get; set; }
        public string BlobUrl { get; set; }
        public string SASToken { get; set; }
        public string FileName { get; set; }
    }
}
