using System;
using System.Collections.Generic;
using System.Text;

namespace MelanomaClassification.Models
{
    public class ModelPrediction
    {
        public string TagId { get; set; }
        public string TagName { get; set; }
        public double Probability { get; set; }
    }
}