using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MelanomaClassification.Models
{
    class ModelCamera
    {
        public async Task<MediaFile> CapturePhoto()
        {

            Console.WriteLine("taking photo");
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                Console.WriteLine("take photo is not supported");
                return null;
            }
            var storeCamerMediaOptions = new StoreCameraMediaOptions()
            {
                SaveToAlbum = true,
            };
            return await CrossMedia.Current.TakePhotoAsync(storeCamerMediaOptions);


        }

    }
}
