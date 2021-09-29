using System;
using System.Collections.Generic;
using System.Text;

namespace MelanomaClassification.Services
{
    public class ClassifierServiceFactory
    {
        private static IClassifierService ConcreteService;
        public static void SetClassifier(IClassifierService service) => ConcreteService = service;

        public IClassifierService GetClassifierService() 
        {
            if (ConcreteService == null) throw new NullReferenceException("Concrete Classifier Service is null");
            return ConcreteService; 
        
        }


    }
}
