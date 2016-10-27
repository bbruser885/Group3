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

        // Test method: Remove all data
        public static void ClearAllData() {
            DB.DropCollection("providers");
            DB.DropCollection("members");
        }


    }
}
