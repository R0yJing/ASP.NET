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
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace MelanomaClassification.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewPhotoGallery : ContentPage
    {
        private ObservableCollection<ModelPredictionWrapper> imageHistory = new ObservableCollection<ModelPredictionWrapper>();
        private ClassifierServiceFactory factory;
        public static ViewPhotoGallery self;
       
        public ICommand Delete
        {
            get { return new Command<ModelPredictionWrapper>(Remove); }
        }
        public volatile bool ImportHasFinished = true;
        public void WaitWhileStillImporting()
        {
            while (!ImportHasFinished)
            {
                System.Threading.Thread.Sleep(50);

            }

        }

        private async Task<bool> AskForPermissions()
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
        }
        public async void Remove(ModelPredictionWrapper self)
        {
            Console.WriteLine("deleting");
            imageHistory.Remove(self);

            DatabaseService.DeleteByIdAsync(self.ParentId);
            //if (deleted) await DisplayAlert("Success", "Deletion succeeded", "OK");

        }
        public ViewPhotoGallery()
        {
            InitializeComponent();

            ImageHistory.ItemsSource = imageHistory;
            pPhotoGallery = new PresenterPhotoGallery(this);
            ImportButton.Clicked += OnImportButtonFunc;
            BindingContext = this;

        }
        public ICommand send
        {
            get { return new Command(() => UserService.UpdateRemote()); }
            set { }
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
                    var wrapper = ImageUtilityService.CreatePredictionWrapper(rawData, predicts);
                    DatabaseService.PutAsync(wrapper);
                    string resultString = wrapper.ToString();

                    await Shell.Current.GoToAsync($"{nameof(ViewResultPage)}?result={resultString}");
                });
            }
        }

        

        public async void OnImportButtonFunc(object sender, EventArgs e)
        {
            ImportHasFinished = false;
            Device.BeginInvokeOnMainThread(async () => await AskForPermissions());
            var service = DependencyService.Get<IMultiPhotoPickerService>();
            service.OpenGallery();
            if (Device.RuntimePlatform == Device.Android)
            {
                MessagingCenter.Unsubscribe<App, List<string>>((App)Xamarin.Forms.Application.Current, "ImagesSelectedAndroid");
                MessagingCenter.Subscribe<App, List<string>>((App)Xamarin.Forms.Application.Current, "ImagesSelectedAndroid", (app, listPaths) => pPhotoGallery.HandlePhotoReceived(listPaths));
                
            }
        }

        protected override void OnDisappearing()
        {
            //unsubscribe listening to the multiphoto selection event, otherwise a memory leak can result.
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<App, List<string>>((App)Xamarin.Forms.Application.Current, "ImagesSelectedAndroid");
            GC.Collect();
        }


        private PresenterPhotoGallery pPhotoGallery;
        
        

        protected override void OnAppearing()
        {
            WaitWhileStillImporting();
            pPhotoGallery.AddNewPredictionsIfAny();
            base.OnAppearing();
        }

        public void AddPrediction(ModelPredictionWrapper data)
        {
            imageHistory.Add(data);
        }


        


    }
}