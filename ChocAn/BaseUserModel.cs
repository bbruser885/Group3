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

        public override string ToString()
        {
            StringBuilder text = new StringBuilder();

            text.Append($"= {Name} =================");
            text.Append(Environment.NewLine);
            text.Append(Environment.NewLine);
            text.Append($"Id: {Id:D9}");
            text.Append(Environment.NewLine);
            text.Append($"Name: {Name}");
            text.Append(Environment.NewLine);
            text.Append("Address:");
            text.Append(Environment.NewLine);
            text.Append($"  Street: {Street}");
            text.Append(Environment.NewLine);
            text.Append($"  City: {City}");
            text.Append(Environment.NewLine);
            text.Append($"  State: {State}");
            text.Append(Environment.NewLine);
            text.Append($"  Zip: {Zip}");
            text.Append(Environment.NewLine);

            return text.ToString();
        }
    }

}
