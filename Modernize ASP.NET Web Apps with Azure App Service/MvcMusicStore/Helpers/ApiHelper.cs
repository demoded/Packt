using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MvcMusicStore.Helpers
{
    public class ApiHelper
    {
        string baseUri;

        public ApiHelper(string _baseUri)
        {
            baseUri = _baseUri;
        }


        public async Task<V> PostAsync<T,V>(string apiAddress, T value)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUri);

            string valueString = string.Empty;
            if (!(value is string))
            {
                var settings = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                };
                valueString = JsonConvert.SerializeObject(value, settings);
            }
            else
            {
                valueString = value as string;
            }
            StringContent content = new StringContent(valueString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(apiAddress, content);

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<V>(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                throw new Exception("Cannot get response from the API: response status " + response.StatusCode);
            }

        }


        public async Task PostAsync<T>(string apiAddress, T value)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUri);
            string valueString = string.Empty;
            if (!(value is string))
            {
                var settings = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                };
                valueString = JsonConvert.SerializeObject(value, settings);
            }
            else
            {
                valueString = value as string;
            }
            StringContent content = new StringContent(valueString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(apiAddress, content);

            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else
            {
                throw new Exception("Cannot get response from the API: response status " + response.StatusCode);
            }

        }


        public async Task<T> GetAsync<T>(string apiAddress)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUri);
            var response = await client.GetAsync(apiAddress);

            if (response.IsSuccessStatusCode)
            {
                var str = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(str);
            }
            else
            {
                throw new Exception("Cannot get response from the API: response status " + response.StatusCode);
            }

        }

        public T Get<T>(string apiAddress)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUri);
            var response = Task.Run(() => client.GetAsync(apiAddress)).Result;
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                throw new Exception("Cannot get response from the API: response status " + response.StatusCode);
            }

        }


        public async Task DeleteAsync(string apiAddress, int id)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUri);
            var response = await client.DeleteAsync(apiAddress + "/" + id);

            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else
            {
                throw new Exception("Cannot get response from the API: response status " + response.StatusCode);
            }

        }
    }
}
