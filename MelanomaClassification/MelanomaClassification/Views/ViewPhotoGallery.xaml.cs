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
        private ObservableCollection<ModelResult> imageHistory = new ObservableCollection<ModelResult>();
        private ClassifierServiceFactory factory;

        public ICommand Delete
        {
            get { return new Command<ModelResult>(Remove); }
        }
        public void Remove(ModelResult self)
        {
            Console.WriteLine("deleting");
            imageHistory.Remove(self);
        }

        public Button btn = new Button();

        public ICommand Classify
        {
            get
            {
                return new Command(async (chosenImageData) =>
                {
                    MemoryStream stream = new MemoryStream((byte[])chosenImageData);
                    ModelPrediction predict = await factory.GetClassifierService().MakePredictions(stream);
                    await Shell.Current.GoToAsync($"{nameof(ViewResultPage)}?resultId={predict.Tag}");
                });
            }
        }
        public ICommand OnImport { get; }
        private PresenterPhotoGallery pPhotoGallery;
        
        public ViewPhotoGallery()
        {
            InitializeComponent();
            
            pPhotoGallery = new PresenterPhotoGallery(this);
            
            OnImport = new Command(() =>
            {
                pPhotoGallery.ImportImage();
            });

            ImageHistory.ItemsSource = imageHistory;
            BindingContext = this;
        }

      
        protected override void OnAppearing()
        {
            base.OnAppearing();
            foreach (ModelResult res in imageHistory)
            {
                res.source = ImageSource.FromStream(() => new MemoryStream(res.imageData));
            }
        }


        public void AddImage(ModelResult res)
        {

            res.source = ImageSource.FromStream(() => new MemoryStream(res.imageData));
            imageHistory.Add(res);

        }


    }
}