using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using MelanomaClassification.Views;
using Plugin.Media.Abstractions;
using MelanomaClassification.Models;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using MelanomaClassification.Services;
using Xamarin.Essentials;
using System.Diagnostics;

namespace MelanomaClassification.Presenters
{
    public class PresenterPhotoGallery
    {
        //private static PresenterCamera pCamera;
        private ViewPhotoGallery vPhotoGallery;
        private ModelPhotoGallery mPhotoGallery;
       
        /*public static void AddPresenterCamera(PresenterCamera pCam)
        {
            pCamera = pCam;
        }*/

        

        public PresenterPhotoGallery(ViewPhotoGallery vPhotoGallery)
        {
            this.vPhotoGallery = vPhotoGallery;
            mPhotoGallery = new ModelPhotoGallery();
        }


        public async Task<bool> ImportImage()
        {
            try
            {
           
                var photoChosen = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = "tmp" });
                if (photoChosen == null) return true;
                var stream = await photoChosen.OpenReadAsync();
                /*var streamCpy = new MemoryStream();
                stream.CopyTo(streamCpy);*/

                Console.WriteLine("stream returned");
                var predictionWrapper = ImageUtilityService.CreatePredictionWrapper(stream);
               
                foreach (var wrapper in vPhotoGallery.GetAllData())
                {
                    if (Enumerable.SequenceEqual(wrapper.ImageData, predictionWrapper.ImageData))
                    {
                        await App.Current.MainPage.DisplayAlert("Duplicate image", "This image is already imported!", "OK");
                    }
                }
                var result = await ClassifierServiceFactory.GetClassifierService().MakePredictions(stream);
                
                predictionWrapper.Predictions = result;
                ModelPhotoGallery.NewPredictions.Add(predictionWrapper);
                DatabaseService.PutAsync(predictionWrapper);
                //await Shell.Current.GoToAsync(nameof(ViewResultPage) + $"?{predictionWrapper}");

            } catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        public void AddNewPredictionsIfAny()
        {
            var newPreds = mPhotoGallery.GetAndRemoveNewPredictions();
            if (newPreds.Count > 0)
            {
                mPhotoGallery.GetAndRemoveNewPredictions().ForEach(Element => vPhotoGallery.AddPrediction(Element));
                vPhotoGallery.ImageHistView.ScrollTo(vPhotoGallery.imageHistory.Count - 1);
            }
        }
        public  interface IViewPhotoGallery
        {
            void OnImport(object sender, EventArgs args);
            void RemoveFromHistory(ModelPredictionWrapper wrapper);
        }
    }
}