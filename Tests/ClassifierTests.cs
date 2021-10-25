using MelanomaClassification;
using MelanomaClassification.Models;
using MelanomaClassification.Presenters;
using MelanomaClassification.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class ClassifierTests
    {
        private readonly ITestOutputHelper output;

        ClassifierServiceFactory fact = new ClassifierServiceFactory();
        private WebClassifierService webService = new WebClassifierService();

        public ClassifierTests(ITestOutputHelper helper) => output = helper;

        
       private string GetAssetsPath(string folder)
        {
            
            var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string path = System.IO.Path.Combine(dir,"Assets/" + folder);
            return path;
            
        }

        private Stream GetStreams(string path)
        {
        
            var fs = File.Open(path, FileMode.Open);

            Assert.NotEmpty(path);

            return fs;
        }
        [Fact]
        public async void CalculatePredictionSpeedRemote()
        {
            WebClassifierService classifier = new WebClassifierService();

            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
          
            string[] benigns = names.Where(name => name.ToLower().Contains("benign")).ToArray();
            var maligns = names.Where(name => name.ToLower().Contains("malign")).ToArray();
            var unknown = names.Where(name => name.ToLower().Contains("unknown")).ToArray();
            
            Stopwatch timer = new Stopwatch();
            var files = new string[9];
            long timeMillis = 0;
            for (int i = 0; i < 9; i++)
            {
                using (var reader = Assembly.GetExecutingAssembly().GetManifestResourceStream(benigns[i]))
                {
                    timer.Start();
                    await classifier.MakePredictions(reader);

                    timer.Stop();
                    timeMillis += timer.ElapsedMilliseconds;
                    
                    output.WriteLine("" + (double) timer.ElapsedMilliseconds / 1000);
                    timer.Reset();
                }

            }
            output.WriteLine("--------");

            for (int i = 0; i < 9; i++)
            {
                using (var reader = Assembly.GetExecutingAssembly().GetManifestResourceStream(maligns[i]))
                {
                    timer.Start();
                    await classifier.MakePredictions(reader);

                    timer.Stop();
                    
                    timeMillis += timer.ElapsedMilliseconds;
                    
                    output.WriteLine("" + (double)timer.ElapsedMilliseconds / 1000);
                    timer.Reset();
                }

            }
            output.WriteLine("--------");
            for (int i = 0; i < 9; i++)
            {
                using (var reader = Assembly.GetExecutingAssembly().GetManifestResourceStream(unknown[i]))
                {
                    timer.Start();
                    await classifier.MakePredictions(reader);
                    
                    timer.Stop();
                    
                    timeMillis += timer.ElapsedMilliseconds;
                    
                    output.WriteLine("" + ((double)timer.ElapsedMilliseconds / 1000));
                    timer.Reset();
                }
            }
            output.WriteLine("!!!!!!!!!!!!!!!!!!");
            output.WriteLine("average = " + ((double) timeMillis / 1000 / 9));
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
        private void AddToConfusionMatrix(double limit, List<int> confusionArrays)
        {
            //add one to the confusion matrices whose probability threshold the current prediction probability (limit)
            //has exceeded
            
            for (int i = 0; i <= (int) (limit * 10); i+= 1)
            {
                try
                {
                    confusionArrays[i]++;
                }
                catch (Exception e)
                {
                    confusionArrays[i] = 1;
                    output.WriteLine("index = " + i);
                    
                }
            }
        }

        [Fact]
        public async void CalculateROC()
        {
            WebClassifierService classifier = new WebClassifierService();

            var malign = @"C:\Users\Roy\Desktop\MobSysDev\BenignMClass\BenchmarkDermoscopic\malign";
            var mFiles = Directory.GetFiles(malign);

            var benign = @"C:\Users\Roy\Desktop\MobSysDev\BenignMClass\BenchmarkDermoscopic\benign";
            var bFiles = Directory.GetFiles(benign);
            var benignFiles = new string[80];
            
            var malignFiles = new string[20];

            for (int i = 0; i < 80; i++)
            {
                benignFiles[i] = bFiles[i];
            }
            for (int i = 0; i<20; i++)
            {
                malignFiles[i] = mFiles[i];
         
            }
            
          
         
            List<int> TP_Maligns = new List<int>(11), FN_Maligns = new List<int>(11), FP_Maligns = new List<int>(11), TN_Maligns = new List<int>(11);
            foreach (var list in new List<int>[] {TP_Maligns, FN_Maligns, FP_Maligns, TN_Maligns })
            {
               for (int i = 0; i <= 10; i++)
                {
                    list.Add(0);
                }
            }

            await GeneratePartialConfusionMatrix(malignFiles, TP_Maligns, FN_Maligns, classifier);
            await GeneratePartialConfusionMatrix(benignFiles, FP_Maligns, TN_Maligns, classifier);
            //await GeneratePartialConfusionMatrix(unknownFiles, FP_Maligns, TN_Maligns, classifier);
            output.WriteLine("percentage of null reference exceptions from postive predictions = " + (((double)pError) / 100));

            output.WriteLine("percentage of null reference exceptions from negative predictions = " + (((double)pError) / 100));

            for (int i = 0; i <= 10; i++)
            {
                output.WriteLine("Threshold = " + (((double) i) / 10) + " \nTPR = " + ((double)TP_Maligns[i] / (TP_Maligns[i] + FN_Maligns[i])));
                output.WriteLine("FPR = " + ((double)FP_Maligns[i] / (FP_Maligns[i] + TN_Maligns[i])));
            }
            

        }
        private void AddToConfusionMatrix(double limit, List<int> confusionMatrixAboveThreshold, List<int> confusionMatrixBelowThreshold)
        {
            //add one to the confusion matrices whose probability threshold the current prediction probability (limit)
            //has exceeded

            for (int i = 0; i <= (int)(limit * 10); i += 1)
            {

                confusionMatrixAboveThreshold[i]++;

            }
            for (int i = (int)(limit * 10) + 1; i <= 10; i++)
            {
                confusionMatrixBelowThreshold[i]++;
            }
        }
        static int pError = 0, nError=0;
        
        private async Task GeneratePartialConfusionMatrix(string[] files, List<int> label1, List<int> label2, IClassifierService classifier)
        {
            for (int i = 0; i < files.Length; i++)
            {
                Stream s = new StreamReader(files[i]).BaseStream;
                try
                {
                    var malignPredictionList = (await classifier.MakePredictions(s)).Where(item => item.TagName.ToLower().Contains("malign")).ToArray();
                    var malign = malignPredictionList[0];
                    AddToConfusionMatrix(malign.Probability, label1,label2);
                }
                catch (ArgumentNullException)
                {
                    if (files[0].ToLower().Contains("malign"))
                    {
                        pError++;
                    }
                    else nError++;
                }
                finally
                {
                    s.Dispose();
                }

            }
        }
        [Fact]
        public async void TestContrast()
        {
         
            string originalPath = @"C:\Users\Roy\Desktop\MobSysDev\ToBeContrastEnhanced";

            string contrast1p2path = @"C:\Users\Roy\Desktop\MobSysDev\AlreadyContrasted_f1.2";
            var originalFiles = Directory.GetFiles(originalPath);

            var contrastedFiles = Directory.GetFiles(contrast1p2path);
            for (int i= 0; i < originalFiles.Length; i++)
            {
                using (var streamRead = new StreamReader(contrastedFiles[i])) {
                    var preds = await webService.MakePredictions(streamRead.BaseStream);
                    var wrapper = new ModelPredictionWrapper
                    {
                        Predictions = preds
                    };
                    output.WriteLine(wrapper.ToString());
                }
                using (var streamRead = new StreamReader(originalFiles[i]))
                {
                    var preds = await webService.MakePredictions(streamRead.BaseStream);
                    var wrapper = new ModelPredictionWrapper
                    {
                        Predictions = preds
                    };
                    output.WriteLine(wrapper.ToString());
                }
            }
        }

        
    
    }
}