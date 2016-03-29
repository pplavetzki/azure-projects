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
using Microsoft.ServiceBus.Messaging;
using System.Configuration;

namespace EventProcessor
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private EventProcessorHost _eventProcessorHost = null;

        public override void Run()
        {
            Trace.TraceInformation("EventProcessor is running");

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

            Trace.TraceInformation("EventProcessor has been started");
            string iotHubConnectionString = ConfigurationManager.ConnectionStrings["iotHubConnection"].ConnectionString;
            string iotHubD2cEndpoint = "messages/events";

            StoreEventProcessor.ServiceBusConnectionString = ConfigurationManager.ConnectionStrings["queueConnection"].ConnectionString;
            StoreEventProcessor.StorageConnectionString = ConfigurationManager.ConnectionStrings["storageConnection"].ConnectionString;

            string eventProcessorHostName = Guid.NewGuid().ToString();

            _eventProcessorHost = new EventProcessorHost(eventProcessorHostName, iotHubD2cEndpoint, EventHubConsumerGroup.DefaultGroupName, iotHubConnectionString, StoreEventProcessor.StorageConnectionString, "messages-events");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("EventProcessor is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            _eventProcessorHost.UnregisterEventProcessorAsync().Wait();

            base.OnStop();

            Trace.TraceInformation("EventProcessor has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            _eventProcessorHost.RegisterEventProcessorAsync<StoreEventProcessor>().Wait();

            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }
    }
}
