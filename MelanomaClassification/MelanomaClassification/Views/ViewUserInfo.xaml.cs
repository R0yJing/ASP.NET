using MelanomaClassification.Presenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MelanomaClassification.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewUserInfo : ContentPage
    {
        private PresenterUserInfo pUserInfo;
        
        public ViewUserInfo()
        {
            pUserInfo = new PresenterUserInfo(this);

            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            pUserInfo.SaveData(Name.Text, DOB.Text, Gender.Text, Ethnicity.Text);

        }
    }
}
