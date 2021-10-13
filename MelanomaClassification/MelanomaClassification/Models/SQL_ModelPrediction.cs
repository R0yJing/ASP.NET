using SQLite;
using System;

namespace MelanomaClassification.Models
{

    [Table("predictionTbl")]
    public class SQL_ModelPrediction
    {
        [NotNull]
        public int Id { get; set; }
        [NotNull]
        public string Username { get => ModelAccountPage.Username; set { } }
        [PrimaryKey]
        public string UniqId { get => ParentId + "/" + Tag; set { } }
        public string ParentId { get => ModelAccountPage.Username + "/" + Id; set { } }
        [NotNull]
        public string Tag { get; set; }
        public string Prob { get; set; }

    }
}