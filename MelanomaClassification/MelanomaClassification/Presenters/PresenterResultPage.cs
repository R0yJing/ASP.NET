using System;
using System.Collections.Generic;
using System.Text;
using MelanomaClassification.Views;
using MelanomaClassification.Models;
using System.IO;
using System.Diagnostics;

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

        public void LoadResult(string prediction)
        {
            Debug.WriteLine("getting files");

            foreach(var path in Directory.GetFiles(".."))
            {
                Debug.WriteLine(path);

            }
            Debug.WriteLine("Previous dir");
            foreach (var path in Directory.GetFiles("../Assets"))
            {
                Debug.WriteLine(path);

            }
            if (prediction == "Malignant")
            {
                vResultPage.SetVisAid(mResultPage.GetImage(mResultPage.warningIcon));
            }
            else
            {
                vResultPage.SetVisAid(mResultPage.GetImage(mResultPage.congratsIcon));
            }
            vResultPage.SetResultText(mResultPage.GetText(prediction));

        }
    }
}
