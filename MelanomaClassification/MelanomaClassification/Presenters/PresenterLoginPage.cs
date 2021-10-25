using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using MelanomaClassification.Models;
using MelanomaClassification.Views;
using MelanomaClassification.Services;

namespace MelanomaClassification.Presenters
{
   

    public class PresenterLoginPage
    {
        private RegisterBindingModel mAccount;
        private ViewLoginPage vLoginPage;
       
        public PresenterLoginPage(ViewLoginPage v)
        {
            vLoginPage = v;
            mAccount = new RegisterBindingModel();
            
        }

        public async void LoginAysnc(string name)
        {
#if DEBUG
            string preferredClassifier = "local";
#else
            string preferredClassifier = await DatabaseService.GetUserPreferredClassifier();
#endif
            if (preferredClassifier == "local")
                ClassifierServiceFactory.SetClassifier(DependencyService.Get<ILocalClassifierService>());
            else ClassifierServiceFactory.SetClassifier(new WebClassifierService());

            //await UserService.LoginAsync(name, password);
            if (preferredClassifier == "local")
                ClassifierServiceFactory.SetClassifier(DependencyService.Get<ILocalClassifierService>());
            else ClassifierServiceFactory.SetClassifier(new WebClassifierService());

            ModelAccountPage.Username = name;
        }

      

        public interface IViewLoginPage
        {
            
        }

    }
}
