using MelanomaClassification.Services;
using MelanomaClassification.Views;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MelanomaClassification
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            DatabaseService.Init();
            UserService.Init();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            Debug.WriteLine("Closing app");
            
            UserService.UpdateRemote();

        }

    }
}
