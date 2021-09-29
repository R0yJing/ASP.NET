using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace MelanomaClassification.Views
{
    public class ViewAccount : ContentPage
    {
       // ObservableCollection<SourceItem> source = new ObservableCollection<SourceStream>();
        public ViewAccount()
        {
          
            Content = new StackLayout
            {
                Children = {
                    new Label { TabIndex = 1, Text = "Welcome to Xamarin.Forms!" },
                    new Label{TabIndex = 0, Text="asf"}
                }
            };
        }
    }
}