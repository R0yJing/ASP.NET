using MelanomaClassification.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace MelanomaClassification.Services
{
    public class ClassifierServiceWrapper
    {
        public static async Task<List<ModelPrediction>> MakePredictions(Stream inStream)
        {
            Bitmap map = new Bitmap(inStream);

            var processed = ImageProcessorService.HistEq(map);
            var memStream = new MemoryStream();
            processed.Save(memStream, ImageFormat.Jpeg);
            var result = await ClassifierServiceFactory.GetClassifierService().MakePredictions(memStream);
            return result;
                
        }
    }
}
