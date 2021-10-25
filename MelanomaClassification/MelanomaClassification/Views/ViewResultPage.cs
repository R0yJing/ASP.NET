using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MelanomaClassification.Models;
using MelanomaClassification.Presenters;
using Xamarin.Forms;

namespace MelanomaClassification.Views
{
    [QueryProperty(nameof(Result), "result")]
    public class ViewResultPage : ContentPage, PresenterResultPage.IViewResultPage
    {
        private const string initVal = "undefined";

        private Image visualAid = new Image();
        private readonly Label resultLbl = new Label { Text = initVal, FontSize=18 };
        private readonly Button reclassifyBtn = new Button { Text = "Re-classify" };



        private PresenterResultPage pResultPage;

        public ViewResultPage()
        {
            pResultPage = new PresenterResultPage(this);
            visualAid.Scale = 2;

            Content = new StackLayout
            {
                Children =
                {
                    new Label(),
                    new Label(),
                    new Label(),
                    visualAid,
                    new Label(),
                    new Label(),
                    resultLbl,
                    reclassifyBtn
                }
            };
            resultLbl.HorizontalTextAlignment = TextAlignment.Center;
            reclassifyBtn.Command = new Command(Reclassify);
            reclassifyBtn.HorizontalOptions = LayoutOptions.CenterAndExpand;
            reclassifyBtn.CornerRadius = 20;
        }

        public async void Reclassify()
        {
            //go back to the camera page or the photogallery page depending on
            //the previous page
            await Navigation.PopAsync();
            
        }

        public string Result
        {

            set
            {
             
                string[] result = value.Split(':');
                var tags = result[0].Split('/');
                var probs = result[1].Split('/');

                pResultPage.LoadResult(tags, probs);
            }
        }

        public void SetVisAid(ImageSource src)
        {
            visualAid.Source = src;
        }

        public void SetResultText(string v)
        {
            
            resultLbl.Text = v;
            
        }


    }
}

