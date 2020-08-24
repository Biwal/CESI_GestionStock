using Negosud.Items;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Negosud.Services
{
    public class RestClient
    {
        private string baseUrl;
        private HttpClient httpClient = new HttpClient();
        private delegate Task<HttpResponseMessage> PerformRequest(string url);
        private delegate Task<HttpResponseMessage> PerformExecution(string requestUri, HttpContent content);

        private static RestClient _instance;
        public static RestClient Instance
        {
            get
            {
                if (_instance == null) _instance = new RestClient("http://localhost:5001/api/");
                return _instance;
            }
        }

        private RestClient(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public async Task<PaginatedResponse<T>> GetAll<T>(string url)
        {
            return await Get<T>(url);
        }

        public async Task<PaginatedResponse<T>> Get<T>(string url, int? id = null)
        {
            HttpResponseMessage response = await PerformRequestAsync(httpClient.GetAsync, baseUrl + url + (id != null ? "/" + id.ToString() : ""));
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PaginatedResponse<T>>(result);
            }

            return new PaginatedResponse<T>();
        }

        public async Task<bool> Post<T>(T obj)
        {
            var content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await PerformExecutionAsync(httpClient.PostAsync, baseUrl + getObjName(obj), content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Put<T>(T obj, int id)
        {
            var content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await PerformExecutionAsync(httpClient.PutAsync, baseUrl + getObjName(obj) + "?id=" + id, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Delete<T>(T obj, int id)
        {
            HttpResponseMessage response = await PerformRequestAsync(httpClient.DeleteAsync, baseUrl + getObjName(obj) + "?id=" + id);
            return response.IsSuccessStatusCode;
        }

        private async Task<HttpResponseMessage> PerformRequestAsync(PerformRequest e, string url)
        {
            try
            {
                return await e(url);
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Exception => " + exception.Message);
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            }
        }

        private async Task<HttpResponseMessage> PerformExecutionAsync(PerformExecution e, string url, HttpContent content)
        {
            try
            {
                return await e(url, content);
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Exception => " + exception.Message);
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            }
        }

        private string getObjName(object obj)
        {
            return obj.GetType().Name.ToLower();
        }
    }
}
