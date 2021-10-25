using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MelanomaClassification.Models
{
    [Table("imageTbl")]
    public class SQL_ModelImage
    {
        public string Date { get; set; }
        [PrimaryKey]
        public int Id { get; set; }
        public byte[] ImageData { get; set; }
        public string ParentId { get => ModelAccountPage.Username + "/" + Id; set { } }
        [NotNull]
        public string Username { get => ModelAccountPage.Username; set { } }
    }
}
