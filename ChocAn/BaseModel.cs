using System;
using System.ComponentModel.DataAnnotations;
using LiteDB;

namespace ChocAn
{
    public abstract class BaseModel
    {
        //  Database connection - All models use this internally
        protected static LiteDatabase DB = new LiteDatabase("data.db");

        // All models should define a default print function
        public abstract void Print();

        // Bootstrap the DB
        public static void InitializeDatabase() {
            // Insert some service records if they don't exist yet
            if (!DB.CollectionExists("services")) {
                Service.Collection.Insert(new Service {
                        Name = "Dietitian consultation",
                        Fee = 150.00
                        });
                Service.Collection.Insert(new Service {
                        Name = "Physical therapy",
                        Fee = 100.00
                        });
                Service.Collection.Insert(new Service {
                        Name = "Sleep laboratory session",
                        Fee = 390.00
                        });
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
    }
}
