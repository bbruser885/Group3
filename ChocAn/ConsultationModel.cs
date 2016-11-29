using System;
using LiteDB;
using System.Text;

namespace ChocAn
{
    public class Consultation : BaseModel
    {
        [BsonIndex]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime Created { get; set; }
        public Service ServiceRecord { get; set; }
        public Member MemberRecord { get; set; }
        public Provider ProviderRecord { get; set; }

        // Automatically set the creation date to now when adding a new consultation
        public Consultation() {
          Created = DateTime.Now;
        }

        public override void Print()
        {
            Console.Write(ToString());
        }

        public override string ToString()
        {
            StringBuilder text = new StringBuilder();

            text.Append("= Consultation =============");
            text.Append(Environment.NewLine);
            text.Append($"Provider: {ProviderRecord.Name}");
            text.Append(Environment.NewLine);
            text.Append($"Member: {MemberRecord.Name}");
            text.Append(Environment.NewLine);
            text.Append($"Service: {ServiceRecord.Name}");
            text.Append(Environment.NewLine);
            text.Append($"Date: {Date:MM-dd-yyyy}");
            text.Append(Environment.NewLine);
            text.Append($"Record Created: {Created}");
            text.Append(Environment.NewLine);

            return text.ToString();
        }

        // `Consultation.Collection` is a LiteDB collection of members
        public static LiteCollection<Consultation> Collection =
            DB.GetCollection<Consultation>("consultation")
                .Include(x => x.MemberRecord)
                .Include(x => x.ProviderRecord)
                .Include(x => x.ServiceRecord);
    }
}

