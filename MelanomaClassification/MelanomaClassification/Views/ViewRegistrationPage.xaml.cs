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
        private UserService userService = new UserService();
       

        public ICommand Register
        {
            get
            {
                return new Command(async () =>
                {
                    //LoadingSymbol.IsRunning = true;
                    Loading.IsVisible = true;
                    LoadingSymbol.IsRunning = true;
                    var successful = await userService.RegisterAsync(EmailEntry.Text, PswdEntry.Text, ConfmPswdEntry.Text);
                    LoadingSymbol.IsRunning = false;
                    Loading.IsVisible = false;
                    Console.WriteLine("registered (or not)");

                    if (successful)
                    {
                        await DisplayAlert("Success!", "Registration successful", "OK");

                    }
                    else await DisplayAlert("Error!", "Registration unsuccessful, probably due to account duplication", "OK");
                });
            }
        }
        public ViewRegistrationPage()
        {
            InitializeComponent();
            BindingContext = this;
        }
    }
}