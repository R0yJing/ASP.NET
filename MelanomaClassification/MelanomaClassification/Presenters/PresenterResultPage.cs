using System;
using System.Collections.Generic;
using System.Text;
using MelanomaClassification.Views;
using MelanomaClassification.Models;

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
