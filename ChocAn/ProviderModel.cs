using System;
using LiteDB;

namespace ChocAn
{
    public class Provider : BaseUser
    {
        // `Provider.Collection` is a LiteDB collection of providers
        public static LiteCollection<Provider> Collection = DB.GetCollection<Provider>("providers");

        public override void Print() {
            Console.WriteLine();
            Console.WriteLine ("= Provider =================");
            Console.WriteLine();
            base.Print();
        }
    }
}

