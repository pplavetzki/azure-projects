using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PareidoliaFileViewer.Models
{
    public class Image
    {
        public string Id { get; set; }
        public string BlobUrl { get; set; }
        public string FileName { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}
