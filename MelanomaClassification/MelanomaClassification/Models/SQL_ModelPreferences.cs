using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MelanomaClassification.Models
{
    [Table("prefTbl")]
    public class SQL_ModelPreferences
    {
        public string Username { get; set; }
        public string classifierUsed { get; set; }
    }
}
