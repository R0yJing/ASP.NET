using System;
using Xamarin.Forms;
using Plugin.Media;
using MelanomaClassification.Models;
using MelanomaClassification.Views;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;

namespace MelanomaClassification.Presenters
{

    public class PresenterCamera
    {
        private IViewCamera vCamera;
        private ModelCamera mCamera;
        
        public PresenterCamera(ViewCamera vCam)
        {
            vCamera = vCam;
            mCamera = new ModelCamera();
        }

      

        public interface IViewCamera
        {
            void TakePhoto();
        }

       
        public async void DisplayPrediction(ModelPrediction predict )
        {
            
            try
            {
               
                await Shell.Current.GoToAsync($"{nameof(ViewResultPage)}?resultId={predict.Tag}");
            }
            catch (Exception e)
            {
                await App.Current.MainPage.DisplayAlert("Alert", e.Message, "OK");

                Console.WriteLine(e.Message);
            }


        }

    }

        

    
}