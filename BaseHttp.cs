using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace TestHttpClient
{
    public class BaseHttp
    {
        public string BaseUri { get; set; } = "";

        protected CancellationTokenSource cts = null;

        // cancel request
        public void Cancel()
        {
            cts?.Cancel();
        }



        private Uri MakeUri(String relativeUri)
        {
            if (BaseUri == null)
                throw new ArgumentNullException("Base Uri is not set!");

            if (relativeUri == null)
                throw new ArgumentNullException("relativeUri");

            return new Uri(BaseUri + relativeUri, UriKind.RelativeOrAbsolute);
        }


        protected async Task<T> GetObject<T>(string urelativeUrl, Dictionary<string, string> extraHeaders = null)
        {
            var jsonData = await GetData(urelativeUrl, extraHeaders);
            // need to add nuget package Newtonsoft.Json
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonData);
        }

        protected async Task<T> PostObject<T>(string urelativeUrl, T o, Dictionary<string, string> extraHeaders = null)
        {
            var jsonData = await PostData(urelativeUrl, Newtonsoft.Json.JsonConvert.SerializeObject(o), "application/json", extraHeaders);
            // need to add nuget package Newtonsoft.Json
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonData);
        }


        

        protected async Task<String> GetData(String relativeUri, Dictionary<string, string> headerParameters = null)
        {

            using (var client = new HttpClient())
            {
                // add any custom request headers 
                if (headerParameters != null && headerParameters.Any())
                {
                    foreach (var key in headerParameters.Keys)
                        client.DefaultRequestHeaders.Add(key, headerParameters[key]);
                }
                // create request
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = MakeUri(relativeUri),
                    Content = null,  //add content here not necessary for get requests
                };

                // create cancelation token if things take too long we can cancel it/ or cancel it after 5 minytes 
                cts = new CancellationTokenSource(5 * 1000 * 60);

                // send request
                using (var response = await client.SendAsync(request, cts.Token))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        return data;
                    }
                    else
                    {  // something gone wrong
                       // may download the request if there is an extra info
                        string serverMsg = "";
                        try
                        {
                            serverMsg = await response.Content.ReadAsStringAsync() ?? "";
                        }
                        catch { }
                        throw new Exception(response.ReasonPhrase.ToString() + serverMsg);
                    }

                }
            }
        }

        


        protected async Task<String> PostData(
                    string relativeUri,
                    string content,
                    string contentType = "application/json",
                    Dictionary<string, string> headerParameters = null
            )
        {
            using (var client = new HttpClient())
            {
                cts = new CancellationTokenSource(5 * 60 * 1000);

                // add any custom request headers 
                if (headerParameters != null && headerParameters.Any())
                {
                    foreach (var key in headerParameters.Keys)
                        client.DefaultRequestHeaders.Add(key, headerParameters[key]);
                }

                var response = await client.PostAsync(MakeUri(relativeUri), new StringContent(content, Encoding.UTF8, contentType), cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    return data;
                }
                else
                {  // something gone wrong
                   // may download the request if there is an extra info
                    string serverMsg = "";
                    try
                    {
                        serverMsg = await response.Content.ReadAsStringAsync() ?? "";
                    }
                    catch { }
                    throw new Exception(response.ReasonPhrase.ToString() + serverMsg);
                }
            }
        }


        
    }
}
