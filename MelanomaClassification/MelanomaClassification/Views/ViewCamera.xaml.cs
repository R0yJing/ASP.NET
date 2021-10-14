using System;
using Xamarin.Forms;
using Plugin.Media;
using MelanomaClassification.Models;
using MelanomaClassification.Views;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using MelanomaClassification.Services;
using System.IO;
using MelanomaClassification.Presenters;

namespace MelanomaClassification.Views
{
    public partial class ViewCamera : ContentPage, PresenterCamera.IViewCamera
    {
        private bool PhotoTaken = true;
        private PresenterCamera pCamera;
        public ViewCamera()
        {
            pCamera = new PresenterCamera(this);
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (PhotoTaken)
            { 
                PhotoTaken = false;
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickPhotoSupported)
                {
                    throw new Exception("No support for camera");
                }
                var storeCamerMediaOptions = new StoreCameraMediaOptions()
                {
                    SaveToAlbum = true,
                };

                MediaFile photoFile = await CrossMedia.Current.TakePhotoAsync(storeCamerMediaOptions);
                if (photoFile == null) return;

                var wrapper = await pCamera.OnTakenPhoto(photoFile);
                
                //release mutex
                await Shell.Current.GoToAsync(nameof(ViewResultPage) + $"?result={wrapper}");
                PhotoTaken = true;

            }
        }
    }
}