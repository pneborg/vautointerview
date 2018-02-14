using Newtonsoft.Json;

namespace vautointerview.Models
{
    public class AnswerResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("totalMilliseconds")]
        public int TotalMilliseconds { get; set; }
    }
}

