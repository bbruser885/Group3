using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
            Console.Write(ToString());
        }

        public override string ToString()
        {
            StringBuilder text = new StringBuilder();
            var delimiter = "\n";

            text.Append(delimiter);
            text.Append(string.Format("= {0} =================" + delimiter, this.GetType().Name));
            text.Append(delimiter);
            text.Append(string.Format("Id: {0}" + delimiter, Id));
            text.Append(string.Format("Name: {0}" + delimiter, Name));
            text.Append("Address:" + delimiter);
            text.Append(string.Format("  Street: {0}" + delimiter, Street));
            text.Append(string.Format("  City: {0}" + delimiter, City));
            text.Append(string.Format("  State: {0}" + delimiter, State));
            text.Append(string.Format("  Zip: {0}" + delimiter, Zip));

            return text.ToString();
        }
    }

}
