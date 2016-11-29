using System;
using System.Collections.Generic;
using LiteDB;

namespace ChocAn
{
    public class Member : BaseUser
    {
        public bool Suspended { get; set; }

        // `Member.Collection` is a LiteDB collection of members
        public static LiteCollection<Member> Collection = DB.GetCollection<Member>("members");

        public override string ToString()
        {
            return base.ToString() +
                   $"Suspended: {(Suspended ? "yes" : "no")}" +
                   Environment.NewLine;
        }
    }
}

