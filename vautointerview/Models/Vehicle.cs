using Newtonsoft.Json;

namespace vautointerview.Models
{
    public class Vehicle
    {
        [JsonProperty("vehicleId")]
        public int? Id { get; set; }

        [JsonProperty("year")]
        public int? Year { get; set; }

        [JsonProperty("make")]
        public string Make { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("dealerId")]
        public int? DealerId { get; set; }

        /// </summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            return $"{this.Id} {this.Make} {this.Model} {this.Year} {this.DealerId}";
        }
    }
}

