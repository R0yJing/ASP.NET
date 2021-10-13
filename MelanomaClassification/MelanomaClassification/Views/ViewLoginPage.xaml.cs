using MelanomaClassification.Presenters;
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
    public partial class ViewLoginPage : ContentPage
    {
        private PresenterLoginPage pLoginPage;
     
        public ICommand OnTapLogin
        {
            get
            {
                return new Command(async () =>
                {
                    Console.WriteLine("logging in");
                    LoggingInIndicator.IsVisible = true;
                    //bool loggedIn = await UserService.LoginAsync(UsernameEntry.Text, PasswordEntry.Text);
                    string preferredClassifier = "local";//await DatabaseService.GetUserPreferredClassifier();
                    if (preferredClassifier == "local")

                        ClassifierServiceFactory.SetClassifier(DependencyService.Get<ILocalClassifierService>());
                    else ClassifierServiceFactory.SetClassifier(new WebClassifierService());

                    LoggingInIndicator.IsVisible = false;
                    if (true)
                    {
                        DependencyService.Get<IKeyboardHelper>().HideKeyboard();
                        Shell.Current.FindByName<ShellContent>("OnLogin").IsVisible = false;
                        if (Shell.Current.FindByName<TabBar>("AfterLogin") == null)
                        {
                            await DisplayAlert("Fatal Error", "After Login not found", "OK");
                        }
                        await Navigation.PushAsync(new ViewAccountPage());
                        Shell.Current.FindByName<TabBar>("AfterLogin").IsVisible = true;

                        
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Unsuccessful log in attempt, please try again", "OK");

                    }
                });
            }

        }

        public ViewLoginPage()
        {
            InitializeComponent();
            BindingContext = this;
            pLoginPage = new PresenterLoginPage(this);
            RegisterLink.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => { await Navigation.PushAsync(new ViewRegistrationPage()); })
            });
        }
    }
}