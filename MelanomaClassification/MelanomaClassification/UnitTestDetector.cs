using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MelanomaClassification
{
    static class UnitTestDetector
    {
        public static bool xunitActive = false;
        static UnitTestDetector()
        {
            
            Assembly[] assm = AppDomain.CurrentDomain.GetAssemblies();
            foreach(var a in assm)
            {
                if (a.FullName.ToLowerInvariant().StartsWith("xunit.framework"))
                {
                    xunitActive = true;
                    break;
                }
            }

        }
    }
    

}
