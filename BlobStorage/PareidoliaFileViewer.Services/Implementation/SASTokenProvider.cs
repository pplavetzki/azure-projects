using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PareidoliaFileViewer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PareidoliaFileViewer.Services.Implementation
{
    public class SASTokenProvider : ISASTokenProvider
    {
        public virtual string GetSASToken()
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var policy = new SharedAccessBlobPolicy();
            policy.Permissions = SharedAccessBlobPermissions.Write;

            policy.SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-10);
            policy.SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(10);

            var container = blobClient.GetContainerReference("files");

            return container.GetSharedAccessSignature(policy);
        }

        public string GetSASToken(CloudBlobContainer container)
        {
            var policy = new SharedAccessBlobPolicy();
            policy.Permissions = SharedAccessBlobPermissions.Write;

            policy.SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-10);
            policy.SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(10);

            return container.GetSharedAccessSignature(policy);
        }
    }
}
