using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MelanomaClassification.Presenters;
using Xamarin.Forms;

namespace MelanomaClassification.Views
{
    [QueryProperty(nameof(Result), "resultId")]
    public class ViewResultPage : ContentPage
    {
        private const string initVal = "undefined";

        private Image visualAid = new Image();
        private readonly Label resultLbl = new Label { Text = initVal };
        private readonly Button reclassifyBtn = new Button { Text = "Re-classify"};

      

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
            await Shell.Current.GoToAsync(nameof(ViewCamera));
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
        }
        public string Result
        {

            set { pResultPage.LoadResult(value); }
           
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

        