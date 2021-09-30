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
            ClassifierServiceFactory.SetClassifier(DependencyService.Get<ILocalClassifierService>());

            BindingContext = this;
            pCamera = new PresenterCamera(this);
            

        }
        public void TakePhoto()
        {
            throw new NotImplementedException();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

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
            var data = new ImageUtilityService().CreatePredictionWrapper(photoFile.GetStream(), predict);
            
            pCamera.DisplayPrediction(predict);
            ModelPhotoGallery.NewPredictions.Add(data);
            


        }


    }
}