using SQLite;
using System;

namespace MelanomaClassification.Models
{

    [Table("predictionTbl")]
    public class SQL_ModelPrediction
    {
        [NotNull]
        public string Username { get; set; }
        [NotNull]
        public byte[] ImageData { get; set; }
        [PrimaryKey] [AutoIncrement]
        public int Id { get; set; }

        public string Tag { get; set; }
        public string Prob;
        public string Date;
        
    }
}