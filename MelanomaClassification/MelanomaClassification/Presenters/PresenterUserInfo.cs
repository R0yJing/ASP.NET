using MelanomaClassification.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace MelanomaClassification.Presenters
{
    public class PresenterUserInfo
    {
        ViewUserInfo vUserInfo;
        
        public PresenterUserInfo(ViewUserInfo v)
        {
            vUserInfo = v;

        }

        internal void SaveData(string name, string dob, string gender, string ethnicity)
        {
            throw new NotImplementedException();
        }
    }
}
