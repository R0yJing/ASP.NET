
using System.IO;
using Xamarin.Forms;

namespace MelanomaClassification.Models
{
    public class ModelPredictionWrapper
    {
        public ModelPrediction Prediction { private get; set; }
        public byte[] ImageData { get; set; }
        public ImageSource Source
        {
            get {
                return ImageData != null
                ? ImageSource.FromStream(() => new MemoryStream(ImageData))
                : throw new System.NullReferenceException("No raw img data found!");
            }
        }
        public string Tag { get { return Prediction?.Tag; } }
        public string Prob { get { return "" + Prediction?.Probability; } }
        public string Date { get; set; }
        public ModelPredictionWrapper self { get {return this;} }
        
    }
}
