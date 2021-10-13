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
            BinaryReader bReader = new BinaryReader(sIn);
            return bReader.ReadBytes((int)sIn.Length);

        }
        private static double ParsePercentage(string perc) => double.Parse(perc.Trim().Replace("%", ""));
        private static ModelPrediction ConvToModelPrediction(SQL_ModelPrediction sqlObj)
        {
            return new ModelPrediction
            {
                TagName = sqlObj.Tag,
                Probability = ParsePercentage(sqlObj.Prob)

            };
        }
        private static List<ModelPrediction> ConvToModelPrediction(List<SQL_ModelPrediction> sqlObj)
        {
            var list = new List<ModelPrediction>();
            sqlObj.ForEach(item => list.Add(ConvToModelPrediction(item)));
            return list;
        }

        public static ModelPredictionWrapper CreatePredictionWrapper(byte[] imageRawData, List<ModelPrediction> predictions = null)
        {

            var data = new ModelPredictionWrapper
            {
                ImageData = imageRawData,
                //Date = DateTime.Now.ToString("dd/MM/yyyy"),
                Predictions = predictions
            };
            return data;
        }

        public static ModelPredictionWrapper CreatePredictionWrapper(Stream stream, List<ModelPrediction> predictions=null)
        {
            byte[] imageRawData = GetByteArrFromImageStream(stream);
            if (predictions == null)
            {
                return CreatePredictionWrapper(imageRawData);
            }
            else return CreatePredictionWrapper(imageRawData, predictions);
        }
        public static List<ModelPredictionWrapper> CreatePredictionWrapper(List<SQL_ModelImage> images, List<SQL_ModelPrediction> predictions)
        {


            var predsMap = new Dictionary<string, List<SQL_ModelPrediction>>();

            var wrappers = new List<ModelPredictionWrapper>();
            predictions.ForEach(item =>
            {
                var listPredicts = new List<SQL_ModelPrediction>();
                predsMap.TryGetValue(item.ParentId, out listPredicts);
                if (listPredicts == null || listPredicts.Count == 0)
                {
                    listPredicts = new List<SQL_ModelPrediction>();
                    predsMap.Add(item.ParentId, listPredicts);
                }
                listPredicts.Add(item);

            });
            images.ForEach(image =>
            {
                var listPredicts = new List<SQL_ModelPrediction>();
                predsMap.TryGetValue(image.ParentId, out listPredicts);
                if (listPredicts == null) throw new Exception("associated preds are none!");
                var wrapper = new ModelPredictionWrapper
                {
                    Predictions = ConvToModelPrediction(listPredicts),
                    ImageData = image.ImageData,
                    ParentId = image.ParentId,

                };
                wrappers.Add(wrapper);
            });
            return wrappers;
        }


    }
}