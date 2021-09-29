using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace MelanomaClassification.Views
{
    public class ViewAboutPage : ContentPage
    {
        public ViewAboutPage()
        {
            Content = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Center,

                Children = {
                    new Label { Text = "About this app", FontSize = 16 },
                    new Label
                    {
                       
                        Text="EZ Melanoma Classifier is a free-to-use\n" +
                        "melanoma self-diagnosis tool trusted by 300,000\n" +
                        "people worldwide. It uses Machine Learning to help\n" +
                        "you detect whether there are signs of melanoma on your\n" +
                        "skin. We hope you enjoy using this app!"
                        
                    },
                    new Label { Text = "Current version ", FontSize = 16 },
                    new Label{ Text = "1.0.0"},
                    new Label {Text = "Developer info"},
                    new Label
                    {
                        Text = "1 Infinity Loop, Apple, California"
                    }

     
                }
            };
        }
    }
}