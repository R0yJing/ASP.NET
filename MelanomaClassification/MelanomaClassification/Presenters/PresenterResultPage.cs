using System;
using System.Collections.Generic;
using System.Text;
using MelanomaClassification.Views;
using MelanomaClassification.Models;
using System.IO;
using System.Diagnostics;
using Xamarin.Forms;

namespace MelanomaClassification.Presenters
{
    
    class PresenterResultPage
    {
        private ViewResultPage vResultPage;
        private ModelResultPage mResultPage;

        public PresenterResultPage(ViewResultPage viewResultPage)
        {
            mResultPage = new ModelResultPage();

            this.vResultPage = viewResultPage;
        }

        public void LoadResult(string [] tags, string[] prob)
        {
            Debug.WriteLine("getting files");
           
            string resultText = "";
            for(int i =0; i < 3; i++)
            {
                resultText += "Probability of " + tags[i] + " : " + (double.Parse(prob[i]) * 100) +"%";
            }
            if (double.Parse(prob[0]) >= 0.5)
            {
                resultText += "This image is most likely " + tags[0];
            }
            else resultText += "Sorry, we can not classify this image";

            vResultPage.SetResultText(resultText);
            var visAid = new Image();
            try
            {
                if (double.Parse(prob[0]) < 0.5 || tags[0].ToLower().Contains("unknown"))
                {
                    visAid.Source = ImageSource.FromFile("../Assets/questionMark.jpg");
                    vResultPage.SetVisAid(visAid);
                }
                else if (tags[0].ToLower().Contains("benign"))
                {
                    visAid.Source = ImageSource.FromFile("../Assets/congratsIcon.png");
                    vResultPage.SetVisAid(visAid);
                } else if (tags[0].ToLower().Contains("malign")){
                    visAid.Source = ImageSource.FromFile("../Assets/warningIcon.png");
                    vResultPage.SetVisAid(visAid);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't load image");
            }
        }
    }
}
