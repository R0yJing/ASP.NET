using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Java.IO;
using Java.Nio;
using Java.Nio.Channels;
using MelanomaClassification.Models;
using MelanomaClassification.Services;
using Plugin.CurrentActivity;
using Xamarin.Forms;

//the DependencyAttribute specifies that platform specific service will be used at runtime
[assembly: Dependency(typeof(MelanomaClassification.Droid.ImageClassifier))]
namespace MelanomaClassification.Droid
{
    public class ImageClassifier : ILocalClassifierService
    {
        //FloatSize is a constant with the value of 4 because a float value is 4 bytes
        private const int FloatSize = 4;
        //PixelSize is a constant with the value of 3 because a pixel has three color channels: Red Green and Blue
        private const int PixelSize = 3;

        [System.Obsolete]
        public async Task<List<ModelPrediction>> MakePredictions(Stream streamIn)
        {


            var image = ImageUtilityService.GetByteArrFromImageStream(streamIn);
            var mappedByteBuffer = GetModelAsMappedByteBuffer();
            var interpreter = new Xamarin.TensorFlow.Lite.Interpreter(mappedByteBuffer);
            
            //To resize the image, we first need to get its required width and height
            var tensor = interpreter.GetInputTensor(0);
           
            var shape = tensor.Shape();

            var width = shape[1];
            var height = shape[2];

            var byteBuffer = GetPhotoAsByteBuffer(image, width, height);
            
            //use StreamReader to import the labels from labels.txt
            var streamReader = new StreamReader(CrossCurrentActivity.Current.Activity.Assets.Open("labels.txt"));
           
            //Transform labels.txt into List<string>
            var labels = streamReader.ReadToEnd().Split('\n').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
            streamReader.Close();
            //Convert our two-dimensional array into a Java.Lang.Object, the required input for Xamarin.TensorFlow.List.Interpreter
            var outputLocations = new float[1][] { new float[labels.Count] };
            var outputs = Java.Lang.Object.FromArray(outputLocations);
           
            interpreter.Run(byteBuffer, outputs);
            var classificationResult = outputs.ToArray<float[]>();

            //Map the classificationResult to the labels and sort the result to find which label has the highest probability
            var classificationModelList = new List<ModelPrediction>();
            for (var i = 0; i < labels.Count; i++)
            {
                var label = labels[i];
                classificationModelList.Add(new ModelPrediction
                {
                    TagName = label,
                    Probability = classificationResult[0][i]
                });
            }
            interpreter.Close();
            classificationModelList.Sort((a, b) => a.Probability < b.Probability ? 1 : -1);
            return classificationModelList;
        }

        //Convert model.tflite to Java.Nio.MappedByteBuffer , the require type for Xamarin.TensorFlow.Lite.Interpreter
        private MappedByteBuffer GetModelAsMappedByteBuffer()
        {
            var assetDescriptor = Android.App.Application.Context.Assets.OpenFd("classifierv2.tflite");
            var inputStream = new FileInputStream(assetDescriptor.FileDescriptor);

            var mappedByteBuffer = inputStream.Channel.Map(FileChannel.MapMode.ReadOnly, assetDescriptor.StartOffset, assetDescriptor.DeclaredLength);
            inputStream.Close();

            return mappedByteBuffer;
        }

        //Resize the image for the TensorFlow interpreter
        private ByteBuffer GetPhotoAsByteBuffer(byte[] image, int width, int height)
        {
            var bitmap = BitmapFactory.DecodeByteArray(image, 0, image.Length);
            var resizedBitmap = Bitmap.CreateScaledBitmap(bitmap, width, height, true);

            var modelInputSize = FloatSize * height * width * PixelSize;
            var byteBuffer = ByteBuffer.AllocateDirect(modelInputSize);
           
            byteBuffer.Order(ByteOrder.NativeOrder());

            var pixels = new int[width * height];
            resizedBitmap.GetPixels(pixels, 0, resizedBitmap.Width, 0, 0, resizedBitmap.Width, resizedBitmap.Height);

            var pixel = 0;

            //Loop through each pixels to create a Java.Nio.ByteBuffer
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var pixelVal = pixels[pixel++];

                    byteBuffer.PutFloat(pixelVal >> 16 & 0xFF);
                    byteBuffer.PutFloat(pixelVal >> 8 & 0xFF);
                    byteBuffer.PutFloat(pixelVal & 0xFF);
                }
            }

            bitmap.Recycle();
            resizedBitmap.Recycle();
            return byteBuffer;
        }
    }


}