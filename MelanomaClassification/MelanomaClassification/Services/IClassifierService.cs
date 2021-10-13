using MelanomaClassification.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MelanomaClassification.Services
{
    
    public interface IClassifierService
    {

        Task<List<ModelPrediction>> MakePredictions(Stream photoStream);

    }
}
