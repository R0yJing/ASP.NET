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
        private const string endpointUrl = "https://australiaeast.api.cognitive.microsoft.com/customvision/v3.0/Prediction/09917653-9816-4ebd-9607-77b22d913077/classify/iterations/Iteration1/image";
        private const string webimage_endpointUrl = "https://australiaeast.api.cognitive.microsoft.com/customvision/v3.0/Prediction/09917653-9816-4ebd-9607-77b22d913077/classify/iterations/Iteration1/url";
        private string predictionKey = "c16aca3a55764d95ac293dc1343e14c3";
     
        public WebClassifierService()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-Key", predictionKey);
        }
        
        public async Task<ModelPrediction> MakePredictions(string Url)
        {
            var UrlObject = new JsonUrl { Url = Url };
            var json = JsonConvert.SerializeObject(UrlObject);
            HttpContent content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync(webimage_endpointUrl, content);
            var responseStr = await response.Content.ReadAsStringAsync();
            var predictionsWrapper = JsonConvert.DeserializeObject<ModelResponse>(responseStr);
            var preds = predictionsWrapper.Predictions;
            return preds[0];

        }
        public async Task<ModelPrediction> MakePredictions(Stream photoStream)
        {
            var content = new ByteArrayContent(ImageUtilityService.GetByteArrFromImageStream(photoStream));

            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            var response = await client.PostAsync(endpointUrl, content);
            var responseStr = await response.Content.ReadAsStringAsync();
            
            ModelResponse resp = JsonConvert.DeserializeObject<ModelResponse>(responseStr);
            Debug.WriteLine("hello");
            //return the most likely result which is at index 0
            ModelPrediction pred = resp.Predictions[0];
            Debug.WriteLine(pred.TagName);
            Debug.WriteLine(pred.TagName);
            Debug.WriteLine(pred.Probability);
            //pred.Tag = Tag.GetTag(int.Parse(pred.TagId));
            return pred;

        }

        private class JsonUrl
        {
            public string Url { get; set; }
        }
    }
}
