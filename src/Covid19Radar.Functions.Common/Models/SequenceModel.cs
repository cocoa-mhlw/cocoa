using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Models
{
    public class SequenceModel
    {
        public string id { get; set; }
        public string PartitionKey { get; set; }
        public int value { get; set; }
    }
}
