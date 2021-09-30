using MelanomaClassification;
using MelanomaClassification.Models;
using MelanomaClassification.Presenters;
using MelanomaClassification.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Xamarin.Forms;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;

        ClassifierServiceFactory fact = new ClassifierServiceFactory();
        private WebClassifierService webService = new WebClassifierService();

        public UnitTest1(ITestOutputHelper helper) => output = helper;

        
       private string GetAssetsPath(string folder)
        {
            
            var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string path = System.IO.Path.Combine(dir,folder);
            return path;
            
        }

        private Stream GetStreams(string path)
        {
        
            var fs = File.Open(path, FileMode.Open);

            Assert.NotEmpty(path);

            return fs;
        }

        private List<Stream> GetMaligns()
        {
            string path = GetAssetsPath("Assets\\Benign");
            List<Stream> lStream = new List<Stream>();
            foreach (var file in Directory.GetFiles(path))
            {
                lStream.Add(File.Open(file, FileMode.Open));
            }
            return lStream;
        }
        [Fact]
        public void OfflinePredict()
        {

            IClassifierService service = DependencyService.Get<ILocalClassifierService>();

            fact.SetClassifierService(service);

            //ModelPrediction pred = await fact.GetClassifierService().MakePredictions();

        }

        [Fact]
        public async void OnlinePredict()
        {
            foreach (Stream s in GetMaligns())
            {
                var prediction = await webService.MakePredictions(s);
                output.WriteLine(prediction.Tag == null ? "null" : prediction.Tag);
                output.WriteLine("" + prediction.Probability);
                output.WriteLine("---------------");
            }
        }
        /*[Fact]
        public async void OnlinePredict()
        {
            WebClassifierService webService = new WebClassifierService();
            output.WriteLine("tag = " + pred.Tag);
            output.WriteLine("Probability is " + pred.Probability);
            Assert.True(true);

        }*/
    }
}