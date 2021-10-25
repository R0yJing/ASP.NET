
using MelanomaClassification.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using System.Net;
using System.Net.Sockets;

namespace MelanomaClassification.Services
{
    public class UserService
    {
        private static string rootUrl
        {
            get
            {   //get virtual ip for android. the port 45455 is read from Conveyor Belt
                if (!UnitTestDetector.xunitActive)
                    return "http://" + "10.0.2.2" + ":45455";
                else return "https://localhost:44332";
            }
            set { }
        }

        private string Token = null;
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        private static string localHostUrl
        {
            get { return "http://localhost:44332"; }
            set { }
        }

        private static HttpClient client;

        public static void Init()
        {
            if (client == null)
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback =
                    (msg, cert, chain, err) => { return true; };
                client = new HttpClient(httpClientHandler);
            }
        }

        
       

        internal static Task<bool> DeleteUserAsync()
        {
            throw new NotImplementedException();
        }

        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //client.DefaultRequestHeaders.Accept.Add(new Me)


      

        private static async void RetrieveFromRemote()
        {
            var response = await client.GetAsync(rootUrl + "/api/UsersPhotos");

            var userData = JsonConvert.DeserializeObject<List<SQL_ModelPrediction>>(await response.Content.ReadAsStringAsync());
            //await DatabaseService.PutAll(userData);
            userData.ForEach(data => DatabaseService.PutAllAsync(userData));


        }


        public static async Task<bool> LoginAsync(string name, string pswd)
        {
            string ip;
            try
            {
                ip = GetLocalIPAddress();
            }
            catch (Exception)
            {
                return false;
            }
            Debug.WriteLine(rootUrl);
            var keyVals = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", name),
                new KeyValuePair<string, string>("password", pswd),
                new KeyValuePair<string, string>("grant_type", "password")
            };

  

            var request = new HttpRequestMessage(HttpMethod.Post,
                rootUrl + "/Token");
            Console.WriteLine(client.DefaultRequestHeaders.ToString());
            request.Content = new FormUrlEncodedContent(keyVals);
            //take request
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                //App.Current.MainPage.DisplayAlert("Error", "Request timed out...", "Ok");
                return false;
            }
            JObject resp = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            var accessToken = resp.Value<string>("access_token");
            ModelAccountPage.AccessToken = accessToken;

            Debug.WriteLine(await response.Content.ReadAsStringAsync());
            return response.IsSuccessStatusCode;

        }

        
        public static async Task<bool> LogOffAsync(string token)
        {

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer", token);
            var resp = await client.PostAsync(rootUrl + "/api/Account/Logout", null);
            return resp.IsSuccessStatusCode;
        }
        public static async Task<bool> RegisterAsync(string username, string pswd, string confmPswd)
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

                var response = await client.PostAsync(rootUrl + "/api/Account/Register", content);
                Console.WriteLine(response.ToString());
                await App.Current.MainPage.DisplayAlert("Title", response.Content.ReadAsStringAsync().ToString(), "OK");
                Console.WriteLine(response.ReasonPhrase);
                Console.WriteLine(response);
                return response.IsSuccessStatusCode;

            }
            catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
                throw e;
            }
        }

        private static class LogoffSettings
        {
            public static string AccessToken { get; set; }
            public static string Username { get; set; }
            public static string Password { get; set; }
        }
    }


}