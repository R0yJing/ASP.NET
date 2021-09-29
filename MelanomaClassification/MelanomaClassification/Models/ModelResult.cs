
using System.IO;
using Xamarin.Forms;

namespace MelanomaClassification.Models
{
    public class ModelResult
    {
        public byte[] imageData { get; set; }
        public ImageSource source { get; set; }
        public string result { get; set; }
        public string date { get; set; }
        public ModelResult self { get {return this;} }
        
    }
}
