using System.Text.RegularExpressions;
using LiteDB;

namespace ChocAn
{
    public abstract class BaseModel
    {
        // Database connection - All models use this internally
        protected static LiteDatabase DB = new LiteDatabase("data.db");

        // All models should define a ToString function
        public abstract override string ToString();

        // Bootstrap the DB
        public static void InitializeDatabase() {
            // Insert some service records if they don't exist yet
            if (!DB.CollectionExists("services")) {
                var serviceSeeds = Regex.Split(Properties.Resources.SeedServices, "\r\n|\r|\n");
                foreach (string line in serviceSeeds)
                {
                    var data = line.Split(',');
                    if (data.Length == 2)
                    {
                        Service.Collection.Insert(new Service
                        {
                            Name = data[0],
                            Fee = float.Parse(data[1])
                        });
                    }
                }
            }
            // Set up mappings between models here. This ensures that
            // relationships between objects come out of the database intact.
            BsonMapper.Global.Entity<Consultation>()
                .DbRef(x => x.ServiceRecord, "services")
                .DbRef(x => x.MemberRecord, "members")
                .DbRef(x => x.ProviderRecord, "providers");
        }

        // Test method: Remove all user data
        public static void ClearUserData() {
            DB.DropCollection("providers");
            DB.DropCollection("members");
        }

        // Test method: Remove all manager data
        public static void ClearManagerData()
        {
            DB.DropCollection("managers");     
        }
    }
}
