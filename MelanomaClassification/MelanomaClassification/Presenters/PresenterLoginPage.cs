using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using MelanomaClassification.Models;
using MelanomaClassification.Views;
namespace MelanomaClassification.Presenters
{
   

    public class PresenterLoginPage
    {
        private RegisterBindingModel mAccount;
        private ViewLoginPage vLogin;
       
        public PresenterLoginPage(ViewLoginPage v)
        {
            vLogin = v;
            mAccount = new RegisterBindingModel();
            
        }

        public void SetUsername(string name) => ModelAccountPage.Username = name;
    }
}
