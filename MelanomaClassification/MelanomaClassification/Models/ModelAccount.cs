using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MelanomaClassification.Models
{
    class ModelAccount
    {
        public string Email { get; set; }
        public string Password { get; set; }
        
        public string ConfirmPassword { get; set; }
    }
}
