using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Storage
{
    public class DeviceData : TableEntity
    {
        public DeviceData(string deviceName, string dateTime)
        {
            this.PartitionKey = deviceName;
            this.RowKey = dateTime;
        }

        public DeviceData() { }

        public double Windspeed { get; set; }

    }
}
