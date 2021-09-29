using MelanomaClassification.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
namespace MelanomaClassification
{

    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {

            InitializeComponent();
            Routing.RegisterRoute(nameof(ViewLoginPage), typeof(ViewLoginPage));
            Routing.RegisterRoute(nameof(ViewCamera), typeof(ViewCamera));
            Routing.RegisterRoute(nameof(ViewAccount), typeof(ViewAccount));
            Routing.RegisterRoute(nameof(ViewRegistrationPage), typeof(ViewRegistrationPage));
            Routing.RegisterRoute(nameof(ViewResultPage), typeof(ViewResultPage));
            Routing.RegisterRoute(nameof(ViewPhotoGallery), typeof(ViewPhotoGallery));

        }

    }
}
