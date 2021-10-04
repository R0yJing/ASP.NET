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
        public async void Remove(ModelPredictionWrapper self)
        {
            Console.WriteLine("deleting");
            imageHistory.Remove(self);
            bool deleted = await DatabaseService.DeleteData(self.Id);
            if (deleted) await DisplayAlert("Success", "Deletion succeeded", "OK");

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
                return new Command(async (chosenImageData) =>
                {

                    ModelPrediction predict = await factory.GetClassifierService().MakePredictions(new MemoryStream((byte[])chosenImageData));
                    await Shell.Current.GoToAsync($"{nameof(ViewResultPage)}?resultId={predict.Tag}");
                });
            }
        }

        public ICommand OnImport { get { return new Command(() => pPhotoGallery.ImportImage()); } }
        
        private PresenterPhotoGallery pPhotoGallery;
        
        public ViewPhotoGallery()
        {
            InitializeComponent();
            
            pPhotoGallery = new PresenterPhotoGallery(this);
            BindingContext = this;
           
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UserService.UpdateRemote();

            pPhotoGallery.AddNewPredictionsIfAny();
        }

        public void AddPrediction(ModelPredictionWrapper data) => imageHistory.Add(data);

        


    }
}