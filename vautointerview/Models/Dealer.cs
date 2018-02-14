using Newtonsoft.Json;
using System.Collections.Generic;

namespace vautointerview.Models
{
    public class Dealer
    {
        [JsonProperty("dealerId")]
        public int? Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("vehicles")]
        public List<Vehicle> Vehicles { get; set; }

        /// </summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            return $"{this.Id} {this.Name}";
        }
    }
}

