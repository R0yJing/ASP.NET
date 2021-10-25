using System;
using System.Collections.Generic;
using System.Text;
using MelanomaClassification.Views;
using MelanomaClassification.Models;
using System.IO;
using System.Diagnostics;
using Xamarin.Forms;
using System.Reflection;

namespace MelanomaClassification.Presenters
{

    public class PresenterResultPage
    {
        private ViewResultPage vResultPage;
        private ModelResultPage mResultPage;

        public PresenterResultPage(ViewResultPage viewResultPage)
        {
            mResultPage = new ModelResultPage();

            this.vResultPage = viewResultPage;
        }

        public void LoadResult(string[] tags, string[] prob)
        {
            Debug.WriteLine("getting files");

            string resultText = "";
            //proability threshold
            if (double.Parse(prob[0]) >= 0.5)
            {
                for (int i = 0; i < prob.Length - 1; i++)
                {
                    double val = (int)(double.Parse(prob[i]) * 10000);
                    var perc2DP = (val / 100).ToString();
                    resultText += "Probability of " + tags[i] + " : " + perc2DP + "%\n";
                }
                resultText += "This image is most likely " + tags[0];
            }
            else resultText += "Sorry, we can not determine the possible category of this image, please try again";

            vResultPage.SetResultText(resultText);
            var visAid = new Image();
            try
            {
                Stream tempStream = null;
                if (double.Parse(prob[0]) < 0.5 || tags[0].ToLower().Contains("unknown"))
                {
                    tempStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MelanomaClassification.Assets.questionMark.jpg");      
                }
                else if (tags[0].ToLower().Contains("benign"))
                {
                    tempStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MelanomaClassification.Assets.congratsIcon.png");
                }
                else if (tags[0].ToLower().Contains("malign"))
                {
                    tempStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MelanomaClassification.Assets.warningIcon.png");
                }

                if (tempStream != null)
                {
                    var src = ImageSource.FromStream(() => tempStream);
                    vResultPage.SetVisAid(src);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't load image");
            }
        }

  
        public interface IViewResultPage
        {
            void Reclassify();
            void SetVisAid(ImageSource img);
            void SetResultText(string text);
        }
    }
}