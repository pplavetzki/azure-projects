using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PareidoliaFileViewer.Services.Interfaces
{
    public interface ISASTokenProvider
    {
        string GetSASToken();
        string GetSASToken(CloudBlobContainer container);
    }
}
