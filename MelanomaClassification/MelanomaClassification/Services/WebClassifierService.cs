using MelanomaClassification.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MelanomaClassification.Services
{

    public class WebClassifierService : IClassifierService
    {
        private HttpClient client;
        private ImageUtilityService imgUtil = new ImageUtilityService();
        private string endpointUrl = "https://australiaeast.api.cognitive.microsoft.com/customvision/v3.0/Prediction/e9d7558c-02aa-4f26-9ce4-b27682dd221a/classify/iterations/Iteration2/image";
        private string predictionKey = "c16aca3a55764d95ac293dc1343e14c3";
     
        public WebClassifierService()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-Key", predictionKey);
        }

        public async Task<ModelPrediction> MakePredictions(Stream photoStream)
        {
                var content = new ByteArrayContent(ImageUtilityService.GetByteArrFromImageStream(photoStream));
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                var response = await client.PostAsync(endpointUrl, content);
                var responseStr = await response.Content.ReadAsStringAsync();
            
            var res = JsonConvert.DeserializeObject<ModelResponse>(responseStr);
            Debug.WriteLine("hello");
            ModelPrediction pred = res.Predictions[res.Predictions.Count - 1];
            Debug.WriteLine(pred.Tag);
            Debug.WriteLine(pred.Tag);
            Debug.WriteLine(pred.Probability);
            //pred.Tag = Tag.GetTag(int.Parse(pred.TagId));
            return pred;

        }
    }
}
