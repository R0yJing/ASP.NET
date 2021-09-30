
using System;
using System.Linq;
using Android.Graphics;
using Org.Tensorflow.Contrib.Android;
using System.IO;
using System.Threading.Tasks;
using Plugin.CurrentActivity;
//code is adapted from https://techcommunity.microsoft.com/t5/windows-dev-appconsult/add-ai-feature-to-xamarin-forms-app/ba-p/318200

using Xamarin.Forms;
using Plugin.Media.Abstractions;
using System.Collections.Generic;
using MelanomaClassification.Models;

//the DependencyAttribute specifies that platform specific service will be used at runtime
[assembly: Dependency(typeof(MelanomaClassification.Droid.ImageClassifier))]
namespace MelanomaClassification.Droid
{
    public class ImageClassifier : ILocalClassifierService
    {
        private static readonly string ModelFile = "classifier.pb";
        private static readonly string LabelFile = "labels.txt";
        private static readonly string InputName = "Placeholder";
        private static readonly string OutputName = "loss";
        private static readonly int InputSize = 224;
        private readonly TensorFlowInferenceInterface _inferenceInterface;
        private readonly string[] _labels;

        public async Task<ModelPrediction> MakePredictions(Stream stream)
        {
            var bitmap = await BitmapFactory.DecodeStreamAsync(stream);

            var floatValues = GetBitmapPixels(bitmap);
            var outputs = new float[_labels.Length];
            _inferenceInterface.Feed(InputName, floatValues, 1, InputSize, InputSize, 3);
            _inferenceInterface.Run(new[] { OutputName });
            _inferenceInterface.Fetch(OutputName, outputs);
            //index is 1 or 0
            var index = Array.IndexOf(outputs, outputs.Max());
            ModelPrediction newPrediction = new ModelPrediction
            {
                Tag = Tag.GetTag(index),
            };
            return newPrediction;

        }
        public ImageClassifier()
        {
            try
            {
                var assets = CrossCurrentActivity.Current.Activity.Assets;

                _inferenceInterface = new TensorFlowInferenceInterface(assets, ModelFile);
                using var sr = new StreamReader(CrossCurrentActivity.Current.Activity.Assets.Open(LabelFile));
                _labels = sr.ReadToEnd().Split("\n").Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }

        }




        private async Task<byte[]> LoadByteArrayFromAssetsAsync(string name)
        {
            using (var s = CrossCurrentActivity.Current.Activity.Assets.Open(name))
            using (var ms = new MemoryStream())
            {
                await s.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return ms.ToArray();
            }
        }



        private static float[] GetBitmapPixels(Bitmap bitmap)
        {   //bitmap dimensions
            var floatValues = new float[InputSize * InputSize * 3];
            using (var scaledBitmap = Bitmap.CreateScaledBitmap(bitmap, InputSize, InputSize, false))
            {
                using (var resizedBitmap = scaledBitmap.Copy(Bitmap.Config.Argb8888, false))
                {
                    var intValues = new int[InputSize * InputSize];
                    //must resize the original bitmap into 224 x 224 in order to send the bitmap to the tf model for processing
                    //the input size is verified via https://netron.app/

                    resizedBitmap.GetPixels(intValues, 0, resizedBitmap.Width, 0, 0, resizedBitmap.Width, resizedBitmap.Height);
                    //the resize bitmap must be converted to binary data as tf model only understands binaries

                    for (int r = 0; r < resizedBitmap.Height; r++)
                    {
                        for (int c = 0; c < resizedBitmap.Width; c++)
                        {
                            int val = intValues[r * resizedBitmap.Width + c];
                            //red
                            floatValues[r * 3 + c * 3] = ((val & 0xFF) - 104);
                            //green
                            floatValues[r * 3 + c * 3 + 1] = ((val & 0xFF) - 117);
                            //blue
                            floatValues[r * 3 + c * 3 + 2] = ((val & 0xFF) - 123);

                        }


                    }
                    resizedBitmap.Recycle();

                }
                scaledBitmap.Recycle();

            }
            return floatValues;
        }


    }

}
