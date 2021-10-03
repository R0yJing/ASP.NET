using MelanomaClassification.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MelanomaClassification.Services
{
    public class ImageUtilityService
    {
        public static byte[] GetByteArrFromImageStream(Stream sIn)
        {
            //16384 = 16 x 1024
            /*var buff = new byte[16384];
            using (MemoryStream ms = new MemoryStream())
            {
               
                int read;
                while ((read = sIn.Read(buff, 0, buff.Length)) > 0)
                {
                    ms.Write(buff, 0, read);
                }
                return ms.ToArray();
            }*/
            BinaryReader bReader = new BinaryReader(sIn);
            return bReader.ReadBytes((int)sIn.Length);
            
        }

        public static ModelPredictionWrapper CreatePredictionWrapper(Stream stream, ModelPrediction predict = null)
        {
            byte[] imageRawData = GetByteArrFromImageStream(stream);

            var data = new ModelPredictionWrapper
            {
                ImageData = imageRawData,
                Date = DateTime.Now.ToString("dd/MM/yyyy"),
                Prediction = predict
            };
            return data;
        }
    }
}
