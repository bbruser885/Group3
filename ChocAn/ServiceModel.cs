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

        public override string ToString()
        {
           StringBuilder text = new StringBuilder();

           text.Append(Environment.NewLine);
           text.Append("= Service ==================");
           text.Append(Environment.NewLine);
           text.Append(string.Format($"Id: {Id}"));
           text.Append(Environment.NewLine);
           text.Append(string.Format($"Name: {Name}"));
           text.Append(Environment.NewLine);
           text.Append(string.Format($"Fee: {Fee:C}"));
           text.Append(Environment.NewLine);

            return text.ToString();
        }
    }
}

