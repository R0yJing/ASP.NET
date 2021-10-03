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


        public async void ImportImage()
        {
            try
            {
                var predict = await mPhotoGallery.ImportPhotoAsync();
                foreach (var wrapper in vPhotoGallery.GetAllData())
                {
                    if (Enumerable.SequenceEqual(wrapper.ImageData, predict.ImageData))
                    {
                        await App.Current.MainPage.DisplayAlert("Duplicate image", "This image is already imported!", "OK");
                        return;
                    }
                }
                vPhotoGallery.AddPrediction(predict);
                DatabaseService.PutAsync(predict);

            } catch(Exception e)
            {
                Console.WriteLine(e.ToString());

            }
        }

        public void AddNewPredictionsIfAny() => 
            mPhotoGallery.GetAndRemoveNewPredictions().ForEach(Element => vPhotoGallery.AddPrediction(Element));

    }
}