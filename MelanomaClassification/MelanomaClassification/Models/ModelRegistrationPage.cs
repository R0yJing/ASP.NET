using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace MelanomaClassification.Models
{
    public class ModelRegistrationPage : INotifyPropertyChanged
    {
        private string email, password, confmpassword;

        public string Email
        {
            get { return Email; }
            set
            {
                email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        public string Password
        {
            get { return Password; }
            set
            {
                password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string ConfmPassword
        {
            get { return ConfmPassword; }
            set
            {
                confmpassword = value;
                OnPropertyChanged(nameof(ConfmPassword));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
