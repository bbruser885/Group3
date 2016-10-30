using System;
using System.ComponentModel.DataAnnotations;
using LiteDB;

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

        public override void Print() {
            Console.WriteLine();
            Console.WriteLine ("= Service ==================");
            Console.WriteLine(string.Format("Id: {0}", Id));
            Console.WriteLine(string.Format("Name: {0}", Name));
            Console.WriteLine(string.Format("Fee: {0:C}", Fee));
            Console.WriteLine();
        }
    }
}

