using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Drawing;
using Xamarin.Essentials;
using MelanomaClassification.Services;
using System.Collections.ObjectModel;

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

            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions());
            var stream = await result.OpenReadAsync(); 
           
            Console.WriteLine("stream returned");
            return new ImageUtilityService().CreatePredictionWrapper(stream);
           
        }

        
    }
}