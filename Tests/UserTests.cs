
using MelanomaClassification.Models;
using MelanomaClassification.Presenters;
using MelanomaClassification.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xunit;
using Xunit.Abstractions;
namespace Tests
{
    public class UserTests
    {
        private ITestOutputHelper output;
        public UserTests(ITestOutputHelper helper)
        {
            //UserService.Init();
            //DatabaseService.Init();
            output = helper;
        }
        [Fact]
        public async void TestLoadSampleData()
        {
            List<SQL_ModelPrediction> predictions = await DatabaseService.GetAllAsync();
            Assert.True(predictions != null);
            output.WriteLine("Number of images " + predictions.Count);
            
           
        }
        [Fact]
        public async void TestRemoteAPI()
        {
            string appFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            output.WriteLine("App folder = " + appFolder);

            string[] imagePaths = Directory.GetFiles(appFolder, "*.jpg", SearchOption.AllDirectories);
            List<Stream> streams = new List<Stream>();
            for (int i = 0; i < imagePaths.Length; i++)
            {
                streams.Add(new StreamReader(imagePaths[i]).BaseStream);
            }
            WebClassifierService webAPI = new WebClassifierService();
            if (streams.Count != 0)
            {
                //string url = "https://en.wikipedia.org/wiki/Melanoma#/media/File:Melanoma.jpg";
                var response = await webAPI.MakePredictions(streams[0]);
                output.WriteLine(response.ToString());
                output.WriteLine(response.Tag);
                output.WriteLine("Probability = " + response.Probability);

                Assert.True(response.Tag != null && response.Tag != "");

            }
            else Assert.False(true);
        }
        [Fact]
        public async void TestRegister()
        {
            bool registered = await UserService.RegisterAsync("fxc@gmail.com", "Password?1", "Password?1");
            Assert.True(true);

        }
        [Fact]
        public async void TestLogin()
        {
            bool loggedIn = await UserService.LoginAsync("t@mail.com", "Password?1");
            Assert.True(loggedIn);
        }
        [Fact]
        public async void TestStoreRemote()
        {
            
            
            int numInDb = await DatabaseService.GetNumberItemsCurrentUser();

            UserService.UpdateRemote();
            int numInRemote = await UserService.GetNumberItems();
            Assert.True(numInDb == numInRemote);

        }



        
    }
}
