
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MelanomaClassification.Services
{
    public class UserService
    {
        private const string portNum =      "60268";
        private string androidRootUrl =     "http://10.0.2.2:"  + portNum;
        private string iOSRootUrl =         "http://localhost:" + portNum;
        private string androidRegisterUrl = "http://10.0.2.2:"  + portNum  + "/api/Account/Register";
        private string iOSApiUrl =          "http://localhost:" + portNum + "/api/Account/Register";
        private HttpClient client;
      
        public UserService()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback =
                (msg, cert, chain, err) => { return true; };
            client = new HttpClient(httpClientHandler);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Accept.Add(new Me)
        } 

        public async Task<bool> LoginAsync(string name, string pswd)
        {
            var keyVals = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", name),
                new KeyValuePair<string, string>("password", pswd),
                new KeyValuePair<string, string>("grant_type", "password")
            };

            var request = new HttpRequestMessage(HttpMethod.Post,
                androidRootUrl + "/Token");
            request.Content = new FormUrlEncodedContent(keyVals);
            //take request
            HttpResponseMessage response = await client.SendAsync(request);
            Debug.WriteLine(await response.Content.ReadAsStringAsync());
            return response.IsSuccessStatusCode;

        }
        public async Task<bool> RegisterAsync(string username, string pswd, string confmPswd)
        {
            var newAccount = new Models.RegisterBindingModel
            {
                Email = username,
                Password = pswd,
                ConfirmPassword = confmPswd
            };
            var json = JsonConvert.SerializeObject(newAccount);
            HttpContent content = new StringContent(json);

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            try
            {

                var response = await client.PostAsync("http://10.0.2.2:60268/api/Account/Register", content);
                Console.WriteLine(response.ToString());

                Console.WriteLine(response.ReasonPhrase);
                Console.WriteLine(response);
                return response.IsSuccessStatusCode;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw e;
            }
        }
    }
}
