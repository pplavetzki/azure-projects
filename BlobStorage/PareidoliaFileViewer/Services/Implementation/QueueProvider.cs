using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using PareidoliaFileViewer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PareidoliaFileViewer.Services.Implementation
{
    public class QueueProvider : IQueueProvider
    {
        public async Task AddThumbMessage(string imageName)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

            string queueName = ConfigurationManager.AppSettings["QueueName"];

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue.
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            await queue.AddMessageAsync(new CloudQueueMessage(imageName));
        }
    }
}
