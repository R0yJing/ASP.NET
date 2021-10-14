
using MelanomaClassification.Models;
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
        private static string rootUrl
        {
            get
            {
                if (!UnitTestDetector.xunitActive)
                    return "http://192.168.1.9:45455";
                else return "https://localhost:44332";
            }
            set { }
        }

        private string Token = null;
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


        public static async Task<int> GetNumberItems()
        {
            var response = client.GetAsync(rootUrl + "/api/ImageData/CurrentUser");
            try
            {
                var str = JsonConvert.DeserializeObject(await response.Result.Content.ReadAsStringAsync()) as string;
                return int.Parse(str);

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return -1;
            }

        }

        internal static Task<bool> DeleteUserAsync()
        {
            throw new NotImplementedException();
        }

        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //client.DefaultRequestHeaders.Accept.Add(new Me)


        public static async void UpdateRemote()
        {
            List<SQL_ModelPrediction> predictions = await DatabaseService.GetUserPredictionDataAsync();
            foreach (var prediction in predictions)
            {
                var json = JsonConvert.SerializeObject(prediction);

                var content = new StringContent(json);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                await client.PostAsync(rootUrl + "/api/ImageData", content);
            }
            
        }

        private static async void RetrieveFromRemote()
        {
            var response = await client.GetAsync(rootUrl + "/api/UsersPhotos");

            var userData = JsonConvert.DeserializeObject<List<SQL_ModelPrediction>>(await response.Content.ReadAsStringAsync());
            //await DatabaseService.PutAll(userData);
            userData.ForEach(data => DatabaseService.PutAllAsync(userData));


        }


        public static async Task<bool> LoginAsync(string name, string pswd)
        {
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

            var resp = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) as LoginResponse;

            Debug.WriteLine(await response.Content.ReadAsStringAsync());
            return response.IsSuccessStatusCode;

        }

        private class LoginResponse
        {
            public string Token { get; set; }
            public string Token_Type { get; set; }
            public long expiresIn { get; set; }
            public string UserName { get; set; }
            public string Issued { get; set; }
            public string Expires { get; set; }
        }

        public static async Task<bool> LogOffAsync()
        {
            HttpResponseMessage response = await client.PostAsync(rootUrl + "/api/Account/Logout", null);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
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
                Console.WriteLine(e.ToString());
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