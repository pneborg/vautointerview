using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace vautointerview.Models
{
    public class DataSet
    {
        [JsonProperty("datasetId")]
        public string Id { get; set; }
    }
}

