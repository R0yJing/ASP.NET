using System;
using Xamarin.Forms;
using Plugin.Media;
using MelanomaClassification.Models;
using MelanomaClassification.Views;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using MelanomaClassification.Services;
using System.IO;

namespace MelanomaClassification.Presenters
{

    public class PresenterCamera
    {
        private ClassifierServiceFactory factory;

        private ViewCamera vCamera;
        private ModelCamera mCamera;

        public PresenterCamera(ViewCamera vCam)
        {
            vCamera = vCam;
            mCamera = new ModelCamera();

        }



        public interface IViewCamera
        {
            //void TakePhoto();
        }


        public async Task<bool> DisplayPrediction(ModelPrediction predict)
        {

            try
            {
                await Shell.Current.GoToAsync($"{nameof(ViewResultPage)}?resultId={predict.TagName}");
            }
            catch (Exception e)
            {
                await App.Current.MainPage.DisplayAlert("Alert", e.Message, "OK");
                Console.WriteLine(e.Message);
                return false;
            }
            return true;

        }

        public async Task<ModelPredictionWrapper> LaunchCameraAsync(MediaFile photoFile)
        {
            
            var predicts = await ClassifierServiceFactory.GetClassifierService().MakePredictions(photoFile.GetStream());
            var wrapper = ImageUtilityService.CreatePredictionWrapper(photoFile.GetStream(), predicts);
            Stream s = photoFile.GetStream();
            var rawData = ImageUtilityService.GetByteArrFromImageStream(s);

            ModelPhotoGallery.StorePhotosTaken(rawData, ToPercentage(predicts[0].Probability),
                                                        ToPercentage(predicts[1].Probability),
                                                        ToPercentage(predicts[2].Probability));
            return wrapper;

        }

        private static string ToPercentage(double v)
        {
            return v.ToString("P");
        }
        private async void ShowResult(ModelPredictionWrapper wrapper)
        {
            string benignProb = wrapper.Predictions[0].Probability.ToString();
            string malignProb = wrapper.Predictions[1].Probability.ToString();
            string unknownProb = wrapper.Predictions[2].Probability.ToString();
            Shell.Current.GoToAsync(nameof(ViewResultPage) + "result={benignProb}");

        }

     
    }




     
}