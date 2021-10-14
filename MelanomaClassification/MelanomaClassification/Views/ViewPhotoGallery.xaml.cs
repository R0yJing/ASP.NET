using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MelanomaClassification.Models;
using System.Windows.Input;
using MelanomaClassification.Presenters;
using System.ComponentModel;
using System.IO;
using MelanomaClassification.Services;
using Plugin.Media;
using System.Diagnostics;
/*using Plugin.Permissions;
using Plugin.Permissions.Abstractions;*/

namespace MelanomaClassification.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewPhotoGallery : ContentPage, PresenterPhotoGallery.IViewPhotoGallery
    {
        public ObservableCollection<ModelPredictionWrapper> imageHistory = new ObservableCollection<ModelPredictionWrapper>();
        public CarouselView ImageHistView
        {
            get => ImageHistory;

        }
        public ICommand Cmd_RemoveFromHistory
        {
            get
            {
                return new Command<ModelPredictionWrapper>(RemoveFromHistory);
            }
            
        }
        public async void RemoveFromHistory(ModelPredictionWrapper photo)
        {
            var result = await DisplayAlert("Confirmation needed", "Are you sure you'd like to remove this photo from Image History?", "Ok", "Cancel");
            if (result)
            {
                Console.WriteLine("deleting");
                imageHistory.Remove(photo);
                try
                {
                    DatabaseService.DeleteByIdAsync(photo.ParentId);
                }
                catch (Exception)
                {
                    Debug.WriteLine("Cannot delete this photo");
                }
            }
        

    }
        public volatile bool ImportHasFinished = true;
        private void WaitWhileStillImporting()
        {
            while (!ImportHasFinished)
            {
                System.Threading.Thread.Sleep(50);

            }

        }

        /*private async Task<bool> AskForPermissions()
        {
            try
            {
                await CrossMedia.Current.Initialize();

                var storagePermissions = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                var photoPermissions = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);
                if (storagePermissions != Plugin.Permissions.Abstractions.PermissionStatus.Granted || photoPermissions != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage, Permission.Photos });
                    storagePermissions = results[Permission.Storage];
                    photoPermissions = results[Permission.Photos];
                }
                var granted = Plugin.Permissions.Abstractions.PermissionStatus.Granted;
                if (storagePermissions != granted || photoPermissions != granted)
                {
                    await DisplayAlert("Permissions Denied!", "Please go to your app settings and enable permissions.", "Ok");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("error. permissions not set. here is the stacktrace: \n" + ex.StackTrace);
                return false;
            }
        }*/
      
        public ViewPhotoGallery()
        {
            InitializeComponent();

            ImageHistory.ItemsSource = imageHistory;
            pPhotoGallery = new PresenterPhotoGallery(this);
            ImportButton.Clicked += OnImport;
            BindingContext = this;

        }
   

        public IEnumerable<ModelPredictionWrapper> GetAllData()
        {
            return imageHistory;
        }

        public ICommand Classify
        {
            get
            {
                //the command parameter takes the prediction wrapper currently displayed 
                //in the DataTemplate in the carousel view
                return new Command(async (mPredictionWrapper) =>
                {
                    byte[] rawData = ((ModelPredictionWrapper)mPredictionWrapper).ImageData;
                    var mStream = new MemoryStream(rawData);

                    var predicts = await ClassifierServiceFactory.GetClassifierService().MakePredictions(mStream);
                    ((ModelPredictionWrapper) mPredictionWrapper).Predictions = predicts;
                    
                    DatabaseService.PutAsync(mPredictionWrapper as ModelPredictionWrapper);
                    string resultString = mPredictionWrapper.ToString();
                    await Shell.Current.GoToAsync($"{nameof(ViewResultPage)}?result={resultString}");
                });
            }
        }



        public async void OnImport(object sender, EventArgs e)
        {
            await pPhotoGallery.ImportImage();
            
            pPhotoGallery.AddNewPredictionsIfAny();
            /*Device.BeginInvokeOnMainThread(async () => await AskForPermissions());
            var service = DependencyService.Get<IMultiPhotoPickerService>();
            service.OpenGallery();
            if (Device.RuntimePlatform == Device.Android)
            {
                MessagingCenter.Unsubscribe<App, List<string>>((App)Xamarin.Forms.Application.Current, "ImagesSelectedAndroid");
                MessagingCenter.Subscribe<App, List<string>>((App)Xamarin.Forms.Application.Current, "ImagesSelectedAndroid", (app, listPaths) => pPhotoGallery.HandlePhotoReceived(listPaths));

            }*/
        }

    


        private PresenterPhotoGallery pPhotoGallery;



        protected override void OnAppearing()
        {
            pPhotoGallery.AddNewPredictionsIfAny();
            base.OnAppearing();
        }

        public void AddPrediction(ModelPredictionWrapper data)
        {
            imageHistory.Add(data);
        }
    }
}