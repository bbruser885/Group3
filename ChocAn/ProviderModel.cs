using System;
using LiteDB;

namespace ChocAn
{
    public class Provider : BaseUser
    {
        // `Provider.Collection` is a LiteDB collection of providers
        public static LiteCollection<Provider> Collection = DB.GetCollection<Provider>("providers");
    }
}

