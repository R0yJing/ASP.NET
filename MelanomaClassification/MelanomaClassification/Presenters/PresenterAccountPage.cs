using MelanomaClassification.Models;
using MelanomaClassification.Services;
using MelanomaClassification.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace MelanomaClassification.Presenters
{
    public class PresenterAccountPage
    {
        private UserService userService = new UserService();
       
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

        internal void DeleteAccount()
        {
            throw new NotImplementedException();
        }

        public async void LogOff()
        {
            bool reallyLoggingOut = await App.Current.MainPage.DisplayAlert("Confirmation needed", "Really logging out?", "Ok", "Cancel");
            if (reallyLoggingOut)
            {
                vAccountPage.LogoutApp();
            }
        }

        public async void DelAcct()
        {
            bool reallyDeletingAcct = await App.Current.MainPage.DisplayAlert("Confirmation needed", "Really logging out?", "Ok", "Cancel");
            if (reallyDeletingAcct)
            {
                vAccountPage.LogoutApp();

            }
        }

        public interface IViewAccountPage
        {
            void LogoutApp();
        }

        public void SetEnableStoringRemote(bool value)
        {
            throw new NotImplementedException();
        }

        public async void UpdateRemoteAsync()
        {
            if (EnableRemote)
            {
                /*List<ModelPredictionWrapper> allItems =
                await DatabaseService.GetAllAsync();
                bool updateSuccess = await userService.UpdateRemoteAsync(allItems);
                if (updateSuccess) await App.Current.MainPage.DisplayAlert("Success!", "Your images are saved remotely", "OK");
*/
            }
        }

    }
    
}
