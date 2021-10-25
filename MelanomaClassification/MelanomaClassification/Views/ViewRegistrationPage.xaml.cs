using MelanomaClassification.Models;
using MelanomaClassification.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MelanomaClassification.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewRegistrationPage : ContentPage
    {
        public ViewRegistrationPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public ICommand Register
        {
            get
            {
                return new Command(async () =>
                {
                    if (PswdEntry.Text != ConfmPswdEntry.Text)
                    {
                        await App.Current.MainPage.DisplayAlert("Password mismatch", "Password mismatch. Please try again", "Ok");
                        return;
                    }
                    //LoadingSymbol.IsRunning = true;
                    Loading.IsVisible = true;
                    LoadingSymbol.IsRunning = true;
                    var successful = await UserService.RegisterAsync(EmailEntry.Text, PswdEntry.Text, ConfmPswdEntry.Text);
                    LoadingSymbol.IsRunning = false;
                    Loading.IsVisible = false;
                    DependencyService.Get<IKeyboardHelper>().HideKeyboard();
                    if (successful)
                    {
                        EmailEntry.Text = PswdEntry.Text = ConfmPswdEntry.Text = "";
                        await DisplayAlert("Success!", "Registration successful", "OK");
                        Shell.Current.GoToAsync("//" +nameof(ViewLoginPage));
                    }
                    else await DisplayAlert("Error!", "Registration unsuccessful, probably due to account duplication", "OK");
                });
            }
        }
        
    }
}