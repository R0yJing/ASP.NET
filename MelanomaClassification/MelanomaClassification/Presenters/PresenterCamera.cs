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


        public async Task<ModelPredictionWrapper> OnTakenPhoto(MediaFile photoFile)
        {
            
            var predicts = await ClassifierServiceFactory.GetClassifierService().MakePredictions(photoFile.GetStream());
            var wrapper = ImageUtilityService.CreatePredictionWrapper(photoFile.GetStream(), predicts);
            Stream s = photoFile.GetStream();
            var rawData = ImageUtilityService.GetByteArrFromImageStream(s);
            //add the newly taken photo to the photo gallery
            ModelPhotoGallery.NewPredictions.Add(wrapper);
            return wrapper;

        }

        private static string ToPercentage(double v)
        {
            return v.ToString("P");
        }
       
    }




     
}