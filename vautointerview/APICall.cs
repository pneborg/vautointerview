using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using vautointerview.Models;

namespace vautointerview
{
    /// <summary>
    /// API Call is mainly responsible for making HTTP calls to the API backend.
    /// </summary>
    public static class APICall<T>
    {
        private static readonly string baseAddress = "http://vautointerview.azurewebsites.net";
        /// <summary>
        /// Generic Method to handle Http Get call for the API
        /// </summary>
        public async static Task<T> GetResult(string apiPath)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseAddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                                new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.GetAsync(apiPath);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        T datasetIdResponse = JsonConvert.DeserializeObject<T>(responseString);
                        return datasetIdResponse;
                    }
                    else
                    {
                        throw new Exception($"Return Code {response.StatusCode}: Error occured during api call execution {apiPath}");
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occured during api call execution {apiPath}", ex);
            }
        }

        /// <summary>
        /// Post an Answer to the API to check the results and get a timing in milliseconds
        /// </summary>
        public async static Task<T> PostAnswer(string apiPath, AnswerPost answer)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseAddress);

                    HttpContent content = new StringContent(answer.ToJson(),
                                                           Encoding.UTF8,
                                                           "application/json");

                    HttpResponseMessage response = await client.PostAsync(apiPath, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        T answerResponse = JsonConvert.DeserializeObject<T>(responseString);
                        return answerResponse;
                    }
                    else
                    {
                        throw new Exception($"Return Code {response.StatusCode}: Error occured during api call execution {apiPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occured during api call execution {apiPath}", ex);
            }
        }


    }
}
