using MelanomaClassification.Models;
using MelanomaClassification.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class DatabaseTests
    {
        private ITestOutputHelper output;
        private string[] paths;
        private string pathBenign = @"C:\Users\Roy\Desktop\MobSysDev\MELANOMA\benign\benign";
        private string pathMalign = @"C:\Users\Roy\Desktop\MobSysDev\MELANOMA\malign\malign";
        private string pathUnknown = @"C:\Users\Roy\Desktop\MobSysDev\MELANOMA\unknown\unknown";

        public DatabaseTests(ITestOutputHelper helper)
        {
            ModelAccountPage.Username = "t@mail.com";
            DatabaseService.Init();
            //string folderPath = Path.Combine(Assembly.GetExecutingAssembly().Location, "Assets/Malign");
            paths = Directory.GetFiles(pathBenign, "*.jpg");

            output = helper;
        }

        [Fact]
        public async void GetData()
        {
            
            int numItems = await DatabaseService.GetNumberImagesCurrentUser();
            output.WriteLine("Number of items = " + numItems);
            Assert.True(numItems != 0);

        }

        [Fact]
        public async void PutData()
        {
            byte[] rawData = ImageUtilityService.GetByteArrFromImageStream(new StreamReader(paths[0]).BaseStream);
            if (rawData.Length == 0)
            {
                output.WriteLine("ImageUtil failed");
                Assert.False(true);
                return;
            }
            int nImagesBefore = await DatabaseService.GetNumberImagesCurrentUser();
            int nPredsBefore = await DatabaseService .GetNumberPredictionsCurrentUser();
            var wrapper = new ModelPredictionWrapper
            {

                ImageData = rawData,
                Predictions = new List<ModelPrediction>
                {
                    new ModelPrediction
                    {
                        Probability = 0.97,
                        TagName = "Malignant",
                    },
                     new ModelPrediction
                    {
                        Probability = 0.02,
                        TagName = "Benign",
                    },
                    new ModelPrediction
                    {
                        Probability = 0.01,
                        TagName = "Unknown",
                    }
                }
            };

            await DatabaseService.PutAsync(wrapper);
            int nImagesAfter = await DatabaseService.GetNumberImagesCurrentUser();
            int nPredsAfter = await DatabaseService.GetNumberPredictionsCurrentUser();
            output.WriteLine("current id " + await DatabaseService.GetNumberImagesCurrentUser());

            Assert.True(nImagesAfter - nImagesBefore == 1 && nPredsAfter - nPredsBefore == 3);
        }
        [Fact]
        public async void PrintData()
        {
            var items = await DatabaseService.GetUserImageDataAsync();
            foreach (var item in items)
            {
                output.WriteLine(item.ParentId);
            }
            output.WriteLine("----------------");
            var preds = await DatabaseService.GetUserPredictionDataAsync();
            foreach (var item in preds)
            {
                output.WriteLine(item.ParentId);
            }
        }
        [Fact]
        public async void DeleteData()
        {
            int nImagesBefore = await DatabaseService.GetNumberImagesCurrentUser();
            int nPredsBefore = await DatabaseService.GetNumberPredictionsCurrentUser();
            string idToDelete = ModelAccountPage.Username + "/" + 
                (await DatabaseService.GetNumberImagesCurrentUser());
            DatabaseService.DeleteByIdAsync(idToDelete);
            int nImagesAfter = await DatabaseService.GetNumberImagesCurrentUser();
            int nPredsAfter = await DatabaseService.GetNumberPredictionsCurrentUser();
            Assert.True(nImagesAfter - nImagesBefore == -1 && nPredsAfter - nPredsBefore == -3);
        }
    }

    
}
