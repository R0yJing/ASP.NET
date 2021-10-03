using System;
using System.Collections.Generic;
using System.Text;

namespace MelanomaClassification.Models
{
    public class ModelResponse
    {
        
        public string Id { get; set; }
        public string Project { get; set; }
        public string Iteration { get; set; }
        public DateTime Created { get; set; }
        public IList<ModelPrediction> Predictions { get; set; }

    }
}
