using MelanomaClassification.Views;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Essentials;
using Xamarin.Forms;
namespace MelanomaClassification
{

    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
          
            InitializeComponent();
          
            //Routing.RegisterRoute(nameof(ViewLoginPage), typeof(ViewLoginPage));
            //Routing.RegisterRoute(nameof(ViewCamera), typeof(ViewCamera));
            //Routing.RegisterRoute(nameof(ViewAccountPage), typeof(ViewAccountPage));
            //Routing.RegisterRoute(nameof(ViewPhotoGallery), typeof(ViewPhotoGallery));
            if (!CrossMedia.Current.IsCameraAvailable)
            {
                CameraTab.IsVisible = false;
            }
            Routing.RegisterRoute(nameof(ViewRegistrationPage), typeof(ViewRegistrationPage));
            Routing.RegisterRoute(nameof(ViewResultPage), typeof(ViewResultPage));
            //Routing.RegisterRoute(nameof(ViewPhotoGallery), typeof(ViewPhotoGallery));

        }

    }
}
