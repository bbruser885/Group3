using System;
using System.ComponentModel.DataAnnotations;

namespace ChocAn
{
    public abstract class BaseUser : BaseModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Name must be <= 50 characters.")]
        public string Name { get; set; }
        [StringLength(25, ErrorMessage = "Street must be <= 25 characters.")]
        public string Street { get; set; }
        [StringLength(14, ErrorMessage = "City must be <= 14 characters.")]
        public string City { get; set; }
        [StringLength(2, ErrorMessage = "State must be 2 characters")]
        public string State { get; set; }
        [RegularExpression(@"^(\d{5})$", ErrorMessage = "Zip must be 5 numbers")]
        public int Zip { get; set; }

        public override void Print()
        {
            Console.WriteLine();
            Console.WriteLine (string.Format("= {0} =================", this.GetType().Name));
            Console.WriteLine();
            Console.WriteLine(string.Format("Id: {0}", Id));
            Console.WriteLine(string.Format("Name: {0}", Name));
            Console.WriteLine("Address:");
            Console.WriteLine(string.Format("  Street: {0}", Street));
            Console.WriteLine(string.Format("  City: {0}", City));
            Console.WriteLine(string.Format("  State: {0}", State));
            Console.WriteLine(string.Format("  Zip: {0}", Zip));
        }

    }

}
