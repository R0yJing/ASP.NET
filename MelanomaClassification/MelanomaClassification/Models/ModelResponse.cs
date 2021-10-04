using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MelanomaClassification.Models
{
    public class ModelResponse
    {
        //[JsonProperty("$id")]
        public string Id { get; set; }

        ///[JsonProperty("$project")]
        public string Project { get; set; }
        //[JsonProperty("$iteration")]
        public string Iteration { get; set; }
        //[JsonProperty("created")]
        public DateTime Created { get; set; }
        //[JsonProperty("$predictions")]
        public IList<ModelPrediction> Predictions { get; set; }

    }
}
