using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MelanomaClassification.Models
{
    public class ModelResultPage
    {
        public string warningIcon
        {
            get { return "../Assets/warningIcon.png";  }
            private set { }
        }

        public string congratsIcon
        {
            get { return "../Assets/congratsIcon.png"; }
            private set { }
        }
        public Image GetImage(string path) {
            return new Image
            {
                Source = ImageSource.FromFile(path)
            };

        }
        public string GetText(string tag)
        {
            return tag == "Benign" ? "Congrats, you have no signs of Melanoma" :
                "Warning, potential signs of malignant melanoma detected in photo. \nSeek treatment now!"; 
        }

       
    }
}
