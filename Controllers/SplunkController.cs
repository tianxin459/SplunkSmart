using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SplunkSmart.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SplunkController : ApiController
    {
        [HttpGet]
        [Route("Splunk/{query=}/{timeEarliest=}")]
        public IHttpActionResult Search(string query,string timeEarliest)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback =    ((sender, certificate, chain, sslPolicyErrors) => true);
            var response = string.Empty;
#if DEBUG
            if (string.IsNullOrEmpty(query))
            {
                query = "sourcetype=dev-AccountOrchestration";
            }
            if (string.IsNullOrEmpty(timeEarliest))
            {
                timeEarliest = "-3h";
            }
            //query = "sourcetype=dev-AccountOrchestration";
            //timeEarliest = "-20h";
#endif
            try
            {
                using (var client = new HttpClient())
                {
                    var url = "https://gdcsplunksh04:8089/services/search/jobs?output_mode=json";
                    var requestBody = new
                    {
                        search = query + "| stats count as EventNumber",
                        earliestTime = timeEarliest
                    };
                    var requestDict = new Dictionary<string, string>();
                    requestDict.Add("search", $"search {query}|stats count as eventNum");
                    requestDict.Add("output_mode", $"json");
                    requestDict.Add("exec_mode", $"oneshot");
                    requestDict.Add("adhoc_search_level", $"fast");
                    requestDict.Add("earliest_time", $"{timeEarliest}");
                    var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(requestDict) };
                    var byteArray = Encoding.ASCII.GetBytes("etian:P@ssword_02");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    var resp = client.SendAsync(req);
                    response = resp.Result.Content.ReadAsStringAsync().Result;


                    //var byteArray = Encoding.ASCII.GetBytes("etian:P@ssword_02");
                    //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //var resp = client.PostAsJsonAsync(url, requestBody);
                    //response = resp.Result.Content.ReadAsStringAsync().Result;

                }
            }
            catch (Exception e)
            {
                throw e;
            }
            JObject obj = JsonConvert.DeserializeObject<JObject>(response);
            var num = obj["results"][0]["eventNum"];

            return Ok(num.Value<string>());

            //Random random = new Random();
            //int randomNumber = random.Next(0, 1000);
            //return Ok(randomNumber);
        }
        
    }
}
