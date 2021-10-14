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

            Content = new StackLayout
            {
                Children =
                {
                    visualAid,
                    resultLbl,
                    reclassifyBtn
                }
            };

            reclassifyBtn.Command = new Command(Reclassify);

        }

        public async void Reclassify()
        {
            //go back to the camera page
            var thisPage = await Navigation.PopAsync();
            
        }

        public string Result
        {

            set
            {
                foreach(var page in Navigation.NavigationStack)
                {
                    Debug.WriteLine(page);
                }
                string[] result = value.Split(':');
                var tags = result[0].Split('/');
                var probs = result[1].Split('/');

                pResultPage.LoadResult(tags, probs);
            }
        }

        public void SetVisAid(Image p)
        {
            visualAid.Source = p.Source;

        }

        public void SetResultText(string v)
        {
            
            resultLbl.Text = v;
        }


    }
}

