using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using Storage;
using System.Text.RegularExpressions;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;

namespace DataParser
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
        {
            log.WriteLine(message);
        }

        public static void ParseDataToTable([BlobTrigger("iot-data/{blobName}")] string input)
        {
            TableBatchOperation batch = new TableBatchOperation();
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            CloudTable table = tableClient.GetTableReference("DeviceInfo");
            table.CreateIfNotExists();

            string pattern = @"{(.*?)}+";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(input);

            for(int i = 0; i < matches.Count; i++)
            {
                Dictionary<string, dynamic> datapoint = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(matches[i].Value);
                var deviceData = new DeviceData(datapoint["deviceId"], Convert.ToString(DateTime.UtcNow.Ticks + i));

                deviceData.Windspeed = Convert.ToDouble(datapoint["windSpeed"]);

                batch.Add(TableOperation.Insert(deviceData));
                if(batch.Count >= 100)
                {
                    table.ExecuteBatch(batch);
                    batch = new TableBatchOperation();
                }
            }

            if(batch.Count > 0)
            {
                table.ExecuteBatch(batch);
            }

            Trace.WriteLine("parsing file: " + input.Take(200));
        }

        private static byte[] ReadFully(Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }
    }
}
