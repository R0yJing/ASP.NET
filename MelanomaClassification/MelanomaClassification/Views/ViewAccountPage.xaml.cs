using MelanomaClassification.Presenters;
using MelanomaClassification.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MelanomaClassification.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewAccountPage : ContentPage, PresenterAccountPage.IViewAccountPage
    {
        private PresenterAccountPage pAccount;
        public ViewAccountPage()
        {
            InitializeComponent();
            AddSwitchListener();
            pAccount = new PresenterAccountPage(this);
            BindingContext = this;
        }
        
        public ICommand LogOff
        {
            get => new Command(() =>
            {
                pAccount.LogOff();
                
            });
           
        }
        public void ShowIndicator()
        {
            LogoutIndicator.IsVisible = true;
            mainPage.IsVisible = false;
        }

        public void HideIndicator()
        {
            LogoutIndicator.IsVisible = false;
            mainPage.IsVisible = true;

        }
        public ICommand DeleteAccount
        {
            get => new Command(() =>
            {
                pAccount.DeleteAccount();

            });

            set { }
        }
        //the reason for the manual guard is that manually setting IsToggled will trigger this function again
        //which we don't want
        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            pAccount.OnToggle(e.Value);
        }

        public void AddSwitchListener() => Switch.Toggled += Switch_Toggled;
        public void SetToggled(bool v)
        {
            Switch.IsToggled = v;
        }
        
    }
}