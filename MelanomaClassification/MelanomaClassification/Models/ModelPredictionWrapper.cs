
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Xamarin.Forms;

namespace MelanomaClassification.Models
{

    public class ModelPredictionWrapper
    {

        public override string ToString()
        {
            string content = "";

            Predictions.ForEach(predict => content += predict.TagName + "/");
            content += ":";
            Predictions.ForEach(predict => content += predict.Probability + "/");
            return content;
        }
        public string Username { get => ModelAccountPage.Username; set { } }
        public List<ModelPrediction> Predictions { get; set; }
        public byte[] ImageData { get; set; }

        public string TagNameBestProb
        {
            get
            {
                try
                {
                    return Predictions[0]?.TagName;
                }
                catch (Exception) { return ""; }
            }
            set { }
        }
        public string ProbBestProb
        {
            get
            {
                try
                {
                    return Predictions[0]?.Probability.ToString("P");
                }
                catch (Exception) { return ""; }
            }
            set { }
        }
        public string TagNameMidProb
        {
            get
            {
                try
                {
                    return Predictions[1]?.TagName;
                }
                catch (Exception e) { return "This image has not been classified yet"; }
            }
            set { }
        }
        public string ProbMidProb
        {
            get
            {
                try
                {
                    return Predictions[1]?.Probability.ToString("P");
                }
                catch (Exception) { return ""; }
            }
            set { }
        }
        public string TagNameLowestProb
        {
            get
            {
                try
                {
                    return Predictions[2]?.TagName;
                }
                catch (Exception e) { return ""; }
            }
            set { }
        }
        public string ProbLowestProb
        {
            get
            {
                try
                {
                    return Predictions[2]?.Probability.ToString("P");
                }
                catch (Exception e) { return ""; }
            }
            set { }
        }


        public ImageSource ASource
        {
            get
            {
                Debug.WriteLine("image data is null?" + ImageData == null);

                return ImageData != null
                ? ImageSource.FromStream(() => new MemoryStream(ImageData))
                : throw new System.NullReferenceException("No raw img data found!");
            }
            set { }
        }

        public string Date { get; set; }
        public string ParentId { get; set; }

    }
}