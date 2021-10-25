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
using System.Windows.Input;

namespace MelanomaClassification.Views
{
    public partial class ViewCamera : ContentPage, PresenterCamera.IViewCamera
    {
        private bool PhotoTaken = true;

        private PresenterCamera pCamera;
        public ViewCamera()
        {

            InitializeComponent();
            pCamera = new PresenterCamera(this);
            BindingContext = this;
        }
        private ICommand TakePhoto
        {
            get => new Command(() =>
            {
                StartCamera();
            });
        }

        private async void StartCamera()
        {
            PhotoTaken = false;

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickPhotoSupported)
            { //this exception is just in case the tab for camera somehow is not hidden from view
                throw new Exception("No support for camera");
            }
            var storeCamerMediaOptions = new StoreCameraMediaOptions()
            {
                SaveToAlbum = true,
            };

            var photoFile = await CrossMedia.Current.TakePhotoAsync(storeCamerMediaOptions);

            if (photoFile == null)
            {
                InfoLabel.Text = "No photo taken...redirecting";
                OnPropertyChanged(nameof(InfoLabel));
                
                await Shell.Current.GoToAsync("//"+nameof(ViewPhotoGallery));
                
            }
            else
            {
                InfoLabel.Text = "Classifying...please wait";
                var wrapper = await pCamera.OnTakenPhoto(photoFile);
                await Shell.Current.GoToAsync(nameof(ViewResultPage) + $"?result={wrapper}");
            }
            InfoLabel.Text = "Please wait while camera is being launched...";

            PhotoTaken = true;
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (PhotoTaken)
            {
                StartCamera();
            }
        }
    }
}