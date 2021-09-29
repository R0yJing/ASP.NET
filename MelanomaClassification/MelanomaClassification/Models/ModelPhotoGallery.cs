using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Drawing;
using Xamarin.Essentials;
using MelanomaClassification.Services;

namespace MelanomaClassification.Models
{
    public class ModelPhotoGallery
    {
       
        Dictionary<Image, Stream> imagesToStreams = new Dictionary<Image, Stream>();

        public Image AddPairToDict( Image image, Stream stream )
        {
            imagesToStreams.Add(image, stream);
            return image;

        }

        
        public async Task<ModelResult> ImportPhotoAsync()
        {
            //MediaPicker.CapturePhotoAsync(new MediaPickerOptions());

            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Pick a photo"
            });

           

            var stream = await result.OpenReadAsync(); 
            MemoryStream savedStream = new MemoryStream();
          
            Console.WriteLine("stream returned");
            
            ModelResult mResult = new ModelResult
            {
                imageData = DependencyService.Get<ImageUtilityService>().GetByteArrFromImageStream(stream)
            };

            return mResult;
        }

        
    }
}