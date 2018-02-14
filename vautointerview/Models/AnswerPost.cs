using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace vautointerview.Models
{
    public class AnswerPost
    {
        //not part of the API model for answer post however this is needed to create the API URI
        [JsonIgnore]
        public string DataSetId { get; set; }

        [JsonProperty("dealers")]
        public List<Dealer> Dealers { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
