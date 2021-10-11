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


        public bool ImportImage()
        {
            
            try
            {
                
                //Console.WriteLine("stream returned");
                //return ImageUtilityService.CreatePredictionWrapper(stream);

                /*foreach (var wrapper in vPhotoGallery.GetAllData())
                {
                    if (Enumerable.SequenceEqual(wrapper.ImageData, predict.ImageData))
                    {
                        await App.Current.MainPage.DisplayAlert("Duplicate image", "This image is already imported!", "OK");
                        return;
                    }
                }*/


            } catch(Exception e)
            {
                Console.WriteLine("Cannot import " + e.ToString());
            }
            vPhotoGallery.ImportHasFinished = true;
            return true;
        }
        public void HandlePhotoReceived(List<string> imagePaths)
        {
            var wrappers = new List<ModelPredictionWrapper>();
            //If we have selected images, put them into the carousel view.
            if (imagePaths.Count > 0)
            {
                Console.WriteLine(imagePaths.ToString());
                foreach (var path in imagePaths)
                {
                    Debug.WriteLine("Processing photo...");
                    wrappers.Add(new ModelPredictionWrapper
                    {
                        Date = DateTime.Now.ToString("mm:HH dd/MM/yyyy"),
                        ASource = ImageSource.FromFile(path)
                    });
                }

                wrappers.ForEach(wrapper => vPhotoGallery.AddPrediction(wrapper));

            }

        }
        public void AddNewPredictionsIfAny() => 
            mPhotoGallery.GetAndRemoveNewPredictions().ForEach(Element => vPhotoGallery.AddPrediction(Element));
         
    }
}