
using System.Diagnostics;
using System.IO;
using Xamarin.Forms;

namespace MelanomaClassification.Models
{
    public class ModelPredictionWrapper
    {
    
        public int Id;
        public string Username { get; set; }
        public ModelPrediction Prediction { private get; set; }
        public byte[] ImageData { get; set; }
        public ImageSource Source
        {
            get {
                Debug.WriteLine("image data is null?" + ImageData == null);

                return ImageData != null
                ? ImageSource.FromStream(() => new MemoryStream(ImageData))
                : throw new System.NullReferenceException("No raw img data found!");
            }
        }
        public string Tag { get { return Prediction?.TagName; } }
        public string Prob { get { return "" + Prediction?.Probability; } }
        public string Date { get; set; }
        public int ItemID { get; set; }
        
    }
}
