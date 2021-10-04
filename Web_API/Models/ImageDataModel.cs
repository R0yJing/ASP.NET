using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;


namespace Web_API.Models
{
    public class ImageDataModel
    {
        [Required]
        public string Username { get; set; }
        //[Required]
        public byte[] ImageData { get; set; }
        [Required]
        public int Id { get; set; }
       
        public string Tag { get; set; }
        public string Prob;
        public string Date;
    }
}