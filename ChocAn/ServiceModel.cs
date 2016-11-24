using System;
using System.ComponentModel.DataAnnotations;
using LiteDB;
using System.Text;

namespace ChocAn
{
    public class Service : BaseModel
    {
        // `Service.Collection` is a LiteDB collection of services
        public static LiteCollection<Service> Collection = DB.GetCollection<Service>("services");

        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Name must be <= 20 characters.")]
        public string Name { get; set; }
        [Required]
        [Range(typeof(Double), "0", "999.99", ErrorMessage = "Fee must be between 0 and 999.99.")]
        public double Fee { get; set; }

 

        public override void Print()
        {
            Console.Write(ToString());
        }

        public override string ToString()
        {
           StringBuilder text = new StringBuilder();
            var delimiter = "\n";

            text.Append(delimiter);
            text.Append(string.Format ("= Service ==================" + delimiter));
            text.Append(string.Format(string.Format("Id: {0}" + delimiter, Id)));
            text.Append(string.Format(string.Format("Name: {0}" + delimiter, Name)));
            text.Append(string.Format(string.Format("Fee: {0:C}" + delimiter, Fee)));
            text.Append(delimiter);

            return text.ToString();
        }
    }
}

