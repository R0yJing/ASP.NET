using MelanomaClassification.Models;
using MelanomaClassification.Services;
using MelanomaClassification.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MelanomaClassification.Presenters
{
    public class PresenterAccountPage
    {
        private Stopwatch timer = new Stopwatch();

        private UserService userService = new UserService();
        public bool Aborted = false;
        public bool EnableRemote { get; set; }
        private ViewAccountPage vAccountPage;
        private ModelAccountPage mAccount = new ModelAccountPage();
        
        public PresenterAccountPage(ViewAccountPage v)
        {
            
            vAccountPage = v;
        }

        internal void UpdatePassword()
        {
            throw new NotImplementedException();
        }

        internal void ChangeDetails()
        {
            throw new NotImplementedException();
        }

        public async void DeleteAccount()
        {
            if (await App.Current.MainPage.DisplayAlert("Confirmation neeeded", "Really deleting this account?", "OK", "I changed my mind"))
            {
                DatabaseService.DeleteCurrentUser();
                HttpClient client;
               
                LogOff();
            }
            
        }

        public async void LogOff()
        {
            bool reallyLoggingOut = await App.Current.MainPage.DisplayAlert("Confirmation needed", "Really logging out?", "Ok", "Cancel");
            
            if (reallyLoggingOut)
            {
                vAccountPage.ShowIndicator();
                StartTimer();
                bool logoffSuccessful = false;

                try {
                    logoffSuccessful = await UserService.LogOffAsync(ModelAccountPage.AccessToken);
                }
                catch (TaskCanceledException)
                {
                    //hasn't logged out. the async task has been canceled
                    vAccountPage.HideIndicator();
                }
                if (logoffSuccessful)
                {
                    vAccountPage.HideIndicator();
                    if (Aborted)
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Error aborting, exiting to Login page!", "Ok");

                    }
                    ModelAccountPage.Username = "";
                    Shell.Current.Navigation.PopToRootAsync();

                    Shell.Current.FindByName<ShellContent>("OnLogin").IsVisible = true;
                    Shell.Current.FindByName<TabBar>("AfterLogin").IsVisible = false;

                }
                else
                {   
                    //logout failure
                    //the user has cancelled the logout, meaning he wants to remain on the Account page

                    if (Aborted) return;
                    vAccountPage.HideIndicator();
                    await App.Current.MainPage.DisplayAlert("Could not log out", "Something bad happened, we couldn't log you out, check internet connection and please try again later", "OK");
                }



            }
        }

        

        public interface IViewAccountPage
        {
        }

        public async void OnToggle(bool value)
        {
            if (value) 
            {
                ClassifierServiceFactory.SetClassifier(new WebClassifierService());
            }
            else 
            {

                ClassifierServiceFactory.SetClassifier(DependencyService.Get<ILocalClassifierService>());
            }
       
        }
        public async void StartTimer()
        {
            Aborted = false;
            timer.Reset();
            timer.Start();
          
            while (timer.ElapsedMilliseconds < 3000) {
                Debug.WriteLine(App.Current.MainPage.ToString());
                if (Shell.Current.FindByName<ShellContent>("OnLogin").IsVisible)
                {
                    return;
                }
                Thread.Sleep(50);
            }

            var ok = await App.Current.MainPage.DisplayAlert("Warning", "Taking a wee while to log out...", "Continue", "Abort");
            if (!ok)
            {
                vAccountPage.HideIndicator();
                Aborted = true;
            }
        }
      
    
        

        
     
    }
    
}
