using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Configuration;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ThumnailRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("ThumnailRole is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            string queueName = ConfigurationManager.AppSettings["QueueName"];

            Trace.TraceInformation("ThumnailRole has been started");

            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the queue client
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            Trace.TraceInformation("Queue is created!");

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();

            

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("ThumnailRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("ThumnailRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            string queueName = ConfigurationManager.AppSettings["QueueName"];
            string imagesContainerName = ConfigurationManager.AppSettings["ImagesBlob"];
            string thumbnailContainerName = ConfigurationManager.AppSettings["ThumbnailsBlob"];

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer imagesContainer = blobClient.GetContainerReference(imagesContainerName);
            CloudBlobContainer thumbnailContainer = blobClient.GetContainerReference(thumbnailContainerName);

            // Retrieve a reference to a queue.
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                foreach (CloudQueueMessage message in queue.GetMessages(10, TimeSpan.FromMinutes(5)))
                {
                    Trace.TraceInformation(message.AsString);
                    var thumbnailName = await ImageProcessor.CreateThumbnail(imagesContainer, thumbnailContainer, message.AsString);
                    // Process all messages in less than 5 minutes, deleting each message after processing.
                    queue.DeleteMessage(message);
                }

                await Task.Delay(1000);
            }
        }
    }
}
