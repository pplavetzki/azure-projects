using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PareidoliaFileViewer
{
    public static class StorageConfig
    {
        public static async Task Configure(string storageConnectionString)
        {
            var account = CloudStorageAccount.Parse(storageConnectionString);
            var client = account.CreateCloudBlobClient();
            var serviceProperties = client.GetServiceProperties();

            //Configure CORS
            serviceProperties.Cors = new CorsProperties();
            serviceProperties.Cors.CorsRules.Add(new CorsRule()
            {
                AllowedHeaders = new List<string>() { "*" },
                AllowedMethods = CorsHttpMethods.Put | CorsHttpMethods.Get | CorsHttpMethods.Head | CorsHttpMethods.Post,
                AllowedOrigins = new List<string>() { "*" },
                ExposedHeaders = new List<string>() { "*" },
                MaxAgeInSeconds = 3600 // 60 minutes
            });

            await client.SetServicePropertiesAsync(serviceProperties);
        }
    }
}
