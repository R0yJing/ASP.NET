using MelanomaClassification.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MelanomaClassification.Models
{
    public class ModelPhotoGallery
    {
       
        Dictionary<Image, Stream> imagesToStreams = new Dictionary<Image, Stream>();
        public static ObservableCollection<ModelPredictionWrapper> NewPredictions = new ObservableCollection<ModelPredictionWrapper>();
        public Image AddPairToDict( Image image, Stream stream )
        {
            imagesToStreams.Add(image, stream);
            return image;

        }

        public ObservableCollection<ModelPredictionWrapper> GetAndRemoveNewPredictions()
        {
            var temp = new ObservableCollection<ModelPredictionWrapper>(NewPredictions);
            NewPredictions.Clear();
            return temp;
        
        }
        public async Task<ModelPredictionWrapper> ImportPhotoAsync()
        {
            //MediaPicker.CapturePhotoAsync(new MediaPickerOptions());

            /*var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions());
            var stream = await result.OpenReadAsync(); 
           */
            return null;
        }

        public static async void StorePhotosTaken(byte[] ImgBytes, string probB, string probM, string probU)
        {
            /*int total = await DatabaseService.GetTotalNumber();
            string dateString = DateTime.Now.ToShortTimeString();
            var image = new SQL_ModelImage
            {
                Date = dateString,
                ImageData = ImgBytes
            };

            var benign = new SQL_ModelPrediction
            {
                Id = total + 1,
                Tag = "Benign",
                Prob = probB
            };
            var malign = new SQL_ModelPrediction
            {
                Id = total + 1,
                Tag = "Malign",
                Prob = probM
            };
            var unknown = new SQL_ModelPrediction
            {
                Id = total + 1,
                Tag = "Unknown",
                Prob = probU
            };
            await DatabaseService.PutAsync(malign);
            await DatabaseService.PutAsync(benign);
            await DatabaseService.PutAsync(unknown);
            await DatabaseService.PutAsync(image);

            Console.WriteLine("Done putasync");
*/
        }
    }
}