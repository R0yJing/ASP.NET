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
           
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = "tmp" });
                if (result == null) return true;
                var stream = await result.OpenReadAsync();

                Console.WriteLine("stream returned");
                var imported = ImageUtilityService.CreatePredictionWrapper(stream);

                foreach (var wrapper in vPhotoGallery.GetAllData())
                {
                    if (Enumerable.SequenceEqual(wrapper.ImageData, imported.ImageData))
                    {
                        await App.Current.MainPage.DisplayAlert("Duplicate image", "This image is already imported!", "OK");
                    }
                }

                
                vPhotoGallery.AddPrediction(imported);
                DatabaseService.PutAsync(imported);

            } catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        public void AddNewPredictionsIfAny() => 
            mPhotoGallery.GetAndRemoveNewPredictions().ForEach(Element => vPhotoGallery.AddPrediction(Element));
        public  interface IViewPhotoGallery
        {
            void OnImport(object sender, EventArgs args);
            void RemovePhoto(ModelPredictionWrapper wrapper);
        }
    }
}