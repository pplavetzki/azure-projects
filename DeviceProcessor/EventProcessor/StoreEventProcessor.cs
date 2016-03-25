using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace EventProcessor
{
    public class StoreEventProcessor : IEventProcessor
    {
        private const int MAX_BLOCK_SIZE = 4 * 1024 * 1024;

        public static string ServiceBusConnectionString;
        public static string StorageConnectionString;

        private CloudBlobClient _blobClient;
        private CloudBlobContainer _blobContainer;
        private QueueClient _queueClient;

        private long currentBlockInitOffset;
        private MemoryStream toAppend = new MemoryStream(MAX_BLOCK_SIZE);

        private Stopwatch stopwatch;
        private TimeSpan MAX_CHECKPOINT_TIME = TimeSpan.FromMinutes(5);

        private decimal MaxWindSpeed = 12.0M;

        public StoreEventProcessor()
        {
            var storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
            _blobClient = storageAccount.CreateCloudBlobClient();
            _blobContainer = _blobClient.GetContainerReference("iot-data");
            _blobContainer.CreateIfNotExists();
            _queueClient = QueueClient.CreateFromConnectionString(ServiceBusConnectionString, "data-queue");
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Trace.TraceInformation("Processor Shutting Down. Partition '{0}', Reason: '{1}'.", context.Lease.PartitionId, reason);

            return Task.FromResult<object>(null);
        }

        public Task OpenAsync(PartitionContext context)
        {
            Trace.TraceInformation("StoreEventProcessor initialized.  Partition: '{0}', Offset: '{1}'", context.Lease.PartitionId, context.Lease.Offset);

            if (!long.TryParse(context.Lease.Offset, out currentBlockInitOffset))
            {
                currentBlockInitOffset = 0;
            }
            stopwatch = new Stopwatch();
            stopwatch.Start();

            return Task.FromResult<object>(null);
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (EventData eventData in messages)
            {
                byte[] data = eventData.GetBytes();
                string messageData = Encoding.UTF8.GetString(data, 0, data.Length);
                TelemetryData message = null;

                if (!string.IsNullOrEmpty(messageData))
                {
                    message = await Task.Run(() => JsonConvert.DeserializeObject<TelemetryData>(messageData));
                }
                else
                {
                    Trace.TraceError("Message from device is empty");
                }

                if (message.WindSpeed > MaxWindSpeed)
                {
                    MaxWindSpeed = message.WindSpeed;

                    var messageId = (string)eventData.SystemProperties["message-id"];

                    var queueMessage = new BrokeredMessage(new MemoryStream(data));
                    queueMessage.MessageId = messageId;
                    queueMessage.Properties["messageType"] = "annoucement";

                    await _queueClient.SendAsync(queueMessage);
                    Trace.TraceInformation("New Max Wind Speed achieved!: {0}", message.WindSpeed);
                }
                
                if (toAppend.Length + data.Length > MAX_BLOCK_SIZE || stopwatch.Elapsed > MAX_CHECKPOINT_TIME)
                {
                    await AppendAndCheckpoint(context);
                }
                await toAppend.WriteAsync(data, 0, data.Length);

                Trace.TraceInformation(string.Format("Message received.  Partition: '{0}', Data: '{1}'", context.Lease.PartitionId, Encoding.UTF8.GetString(data)));
            }
         
            await context.CheckpointAsync();
        }

        private async Task AppendAndCheckpoint(PartitionContext context)
        {
            var blockIdString = String.Format("startSeq:{0}", currentBlockInitOffset.ToString("0000000000000000000000000"));
            var blockId = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(blockIdString));
            toAppend.Seek(0, SeekOrigin.Begin);
            byte[] md5 = MD5.Create().ComputeHash(toAppend);
            toAppend.Seek(0, SeekOrigin.Begin);

            var blobName = String.Format("device-data-{0}", context.Lease.PartitionId);
            var currentBlob = _blobContainer.GetBlockBlobReference(blobName);

            if (await currentBlob.ExistsAsync())
            {
                await currentBlob.PutBlockAsync(blockId, toAppend, Convert.ToBase64String(md5));
                var blockList = await currentBlob.DownloadBlockListAsync();
                var newBlockList = new List<string>(blockList.Select(b => b.Name));

                if (newBlockList.Count() > 0 && newBlockList.Last() != blockId)
                {
                    newBlockList.Add(blockId);
                    Trace.TraceInformation(String.Format("Appending block id: {0} to blob: {1}", blockIdString, currentBlob.Name));
                }
                else
                {
                    Trace.TraceInformation(String.Format("Overwriting block id: {0}", blockIdString));
                }
                await currentBlob.PutBlockListAsync(newBlockList);
            }
            else
            {
                await currentBlob.PutBlockAsync(blockId, toAppend, Convert.ToBase64String(md5));
                var newBlockList = new List<string>();
                newBlockList.Add(blockId);
                await currentBlob.PutBlockListAsync(newBlockList);

                Trace.TraceInformation(String.Format("Created new blob", currentBlob.Name));
            }

            toAppend.Dispose();
            toAppend = new MemoryStream(MAX_BLOCK_SIZE);

            // checkpoint.
            await context.CheckpointAsync();
            Trace.TraceInformation(String.Format("Checkpointed partition: {0}", context.Lease.PartitionId));

            currentBlockInitOffset = long.Parse(context.Lease.Offset);
            stopwatch.Restart();
        }
    }
}
