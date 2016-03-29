using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventProcessor
{
    [Serializable()]
    public class TelemetryData
    {
        public string DeviceId { get; set; }
        public double WindSpeed { get; set; }
    }
}
