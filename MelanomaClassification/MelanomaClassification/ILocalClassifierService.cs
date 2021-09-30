using MelanomaClassification.Models;
using MelanomaClassification.Services;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MelanomaClassification
{
    //this interface is still used as at runtime a platform specific image classifier is obtained
    public interface ILocalClassifierService : IClassifierService
    {
        
    }

    public class ResultText
    {
        public static string MalignResult
        {
            get
            {
                return "Warning, photo is classified as malignant. Seek treatment from a medical specialist now!";
            }
            set { }
        }

        public static string BenignResult
        {
            get
            {
                return "Congratulations, no malignant features were detected in the photo!";
            }
            set { }
        }
    }
    public struct Tag
    {
        public static string GetTag(int idx)
        {
            return idx == 0 ? "Benign" : "Malignant";
        }
        
    }
}
