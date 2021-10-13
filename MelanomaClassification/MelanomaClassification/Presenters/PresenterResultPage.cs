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
            
            if (double.Parse(prob[0]) >= 0.5)
            {
                for (int i = 0; i < prob.Length - 1; i++)
                {
                    resultText += "Probability of " + tags[i] + " : " + (double.Parse(prob[i]) * 100) + "%";
                }
                resultText += "This image is most likely " + tags[0];
            }
            else resultText += "Sorry, we can not determine the possible category of this image, please try again";

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

                    Stream tempStream = this.ConvertPathtoStream("../Assets/congratsIcon.png");
                    if (tempStream != null)
                    {
                        var src = ImageSource.FromStream(() => tempStream);
                        visAid.Source = src;

                        vResultPage.SetVisAid(visAid);
                    }
                }
                else if (tags[0].ToLower().Contains("malign"))
                {
                    visAid.Source = ImageSource.FromFile("../Assets/warningIcon.png");
                    vResultPage.SetVisAid(visAid);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't load image");
            }
        }

        private Stream ConvertPathtoStream(string path)
        {
            if (!File.Exists(path)) return null;
            var memStream = new MemoryStream();
            System.IO.File.OpenRead(path).CopyTo(memStream);
            return memStream;
        }
        public interface IViewResultPage
        {
            void Reclassify();
            void SetVisAid(Image img);
            void SetResultText(string text);
        }
    }
}