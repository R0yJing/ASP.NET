using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using MelanomaClassification.Views;
using Plugin.Media.Abstractions;
using MelanomaClassification.Models;
using System.IO;
using System.Threading.Tasks;

namespace MelanomaClassification.Presenters
{
    public class PresenterPhotoGallery
    {
        //private static PresenterCamera pCamera;
        private ViewPhotoGallery vPhotoGallery;
        private ModelPhotoGallery mPhotoGallery;

        /*public static void AddPresenterCamera(PresenterCamera pCam)
        {
            pCamera = pCam;
        }*/

        

        public PresenterPhotoGallery(ViewPhotoGallery vPhotoGallery)
        {
            this.vPhotoGallery = vPhotoGallery;
            mPhotoGallery = new ModelPhotoGallery();

         
        }


        public async void ImportImage()
        {
            try
            {
                vPhotoGallery.AddImage(await mPhotoGallery.ImportPhotoAsync());
            } catch(Exception e)
            {
                Console.WriteLine(e.ToString());

            }
        }

       
    }
}