using System;
using LiteDB;

namespace ChocAn
{
  public class Consultation : BaseModel
  {
    public DateTime Date { get; set; }
    public DateTime Created { get; }
    public Service ServiceRecord { get; set; }
    public Member MemberRecord { get; set; }
    public Provider ProviderRecord { get; set; }

    // Automatically set the creation date to now when adding a new consultation
    public Consultation() {
      Created = DateTime.Now;
    }

    public override void Print() {
      Console.WriteLine();
      Console.WriteLine("= Consultation =============");
      Console.WriteLine(string.Format("Provider: {0}", ProviderRecord.Name));
      Console.WriteLine(string.Format("Member: {0}", MemberRecord.Name));
      Console.WriteLine(string.Format("Service: {0}", ServiceRecord.Name));
      Console.WriteLine(string.Format("Consultaion Date: {0}", Date.ToString("MM-dd-yyyy")));
      Console.WriteLine(string.Format("Record Created: {0}", Created));
    }

    // `Consultation.Collection` is a LiteDB collection of members
    public static LiteCollection<Consultation> Collection =
      DB.GetCollection<Consultation>("consultation")
      .Include(x => x.MemberRecord)
      .Include(x => x.ProviderRecord)
      .Include(x => x.ServiceRecord);
  }
}

