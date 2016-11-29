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
            var delimiter = "\n";

            text.Append(delimiter);
            text.Append("= Consultation =============");
            text.Append(string.Format("Provider: {0}", ProviderRecord.Name));
            text.Append(string.Format("Member: {0}", MemberRecord.Name));
            text.Append(string.Format("Service: {0}", ServiceRecord.Name));
            text.Append(string.Format("Consultaion Date: {0}", Date.ToString("MM-dd-yyyy")));
            text.Append(string.Format("Record Created: {0}", Created));

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

