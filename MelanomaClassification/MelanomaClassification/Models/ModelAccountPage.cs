using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
namespace MelanomaClassification.Models
{
    public class ModelAccountPage
    {
       
        public static string Username { get; set; }
        public static string Password { get; internal set; }
        public static string AccessToken { get; internal set; }
    }
}
