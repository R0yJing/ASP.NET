using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MelanomaClassification.Presenters;
using System.Threading.Tasks;
using System.Linq;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using MelanomaClassification.Models;
using System.Collections.Generic;
using MelanomaClassification.Services;

namespace MelanomaClassification.Views
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewCamera : ContentPage, PresenterCamera.IViewCamera
    {
        private PresenterCamera pCamera;
        private ClassifierServiceFactory factory = new ClassifierServiceFactory();
        public ViewCamera()
        {
            InitializeComponent();
            ClassifierServiceFactory.SetClassifier(DependencyService.Get<ILocalImageClassifierService>());

            BindingContext = this;
            
            takePhotoBtn.Clicked += (async (object sender, EventArgs args) =>
            {

                Console.WriteLine("taking photo");
                _ = await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    Console.WriteLine("take photo is not supported");
                    return;
                }
                var storeCamerMediaOptions = new StoreCameraMediaOptions()
                {
                    SaveToAlbum = true,
                };
                MediaFile photoFile = await CrossMedia.Current.TakePhotoAsync(storeCamerMediaOptions);
                var predict = await factory.GetClassifierService().MakePredictions(photoFile.GetStream());
                pCamera.DisplayPrediction(predict);

            });

            pCamera = new PresenterCamera(this);
            

        }
        public void TakePhoto()
        {
            throw new NotImplementedException();
        }


    }
}