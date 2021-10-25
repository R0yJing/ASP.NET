using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MelanomaClassification.Droid
{
    public class Tests
    {
        private ImageClassifier classifier = new ImageClassifier();

        private Stopwatch watch = new Stopwatch();

        private string GetAssetsPath(string folder)
        {
            Console.WriteLine("getting images from " + folder);
            var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Console.WriteLine(dir);

            string path = System.IO.Path.Combine(dir, folder);
            return path;

        }

        private Stream GetStreams(string path)
        {

            var fs = File.Open(path, FileMode.Open);
            
            return fs;
        }

        private List<Stream> GetMaligns()
        {
            string path = @"Assets/Malign";
            List<Stream> lStream = new List<Stream>();
            string folderMalign = Path.Combine(Directory.GetCurrentDirectory(), path);

            Console.WriteLine(Directory.GetCurrentDirectory());
            string[] files = null;
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var assmName = assembly.GetName().Name;
            var maligns = new List<string>();
            foreach (var res in assembly.GetManifestResourceNames())
            {
                if (res.Contains("ISIC"))
                {
                    Console.WriteLine(res);
                    lStream.Add(File.Open(res, FileMode.Open));
                }
            }


            try
            {
                foreach (var file in files )
                {
                    lStream.Add(File.Open(file, FileMode.Open));
                }
            } catch (Exception e)
            {
                Console.WriteLine("error retrieving files");

            }
            return lStream;
        }

        public async void OfflinePredict()
        {
            foreach (Stream s in GetMaligns())
            {
               
                var prediction = await classifier.MakePredictions(s);

                Log.Info("OfflinePredict", "Prediction tag = " + prediction.Tag);
                Log.Info("OfflinePredict", "Probabilty = " + prediction.Probability);

            }
        }

    
    }
}
