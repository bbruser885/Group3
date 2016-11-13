using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ChocAn
{
    public class Controller
    {

        public static void PrintAllProviders()
		{
            View.PrintAll(Provider.Collection.FindAll());
        }

        public static void PrintAllMembers()
		{
            View.PrintAll(Member.Collection.FindAll());
        }

        public static void PrintAllManagers()
		{
            View.PrintAll(Manager.Collection.FindAll());
        }

        public static void PrintAllServices()
		{
            View.PrintAll(Service.Collection.FindAll());
        }

        public static void PrintAllConsultations()
		{
            View.PrintAll(Consultation.Collection.FindAll());
        }

        public static void CreateMember()
		{
            Member.Collection.Insert(View.ReadMember());
        }

        public static void CreateProvider()
		{
            Provider.Collection.Insert(View.ReadProvider());
        }

        public static void CreateManager()
        {
            Manager.Collection.Insert(View.ReadManager());
        }

        public static void CreateConsultation()
		{
            Provider provider = View.ReadProviderById();
            Member member = View.ReadMemberById();
            Service service = View.ReadServiceById();
            DateTime date = View.ReadDateTime("Date of consultation");
            Consultation.Collection.Insert(new Consultation {
                ProviderRecord = provider,
                MemberRecord = member,
                ServiceRecord = service,
                Date = date
            });
        }


        public static void ClearUserData()
        {
            BaseModel.ClearUserData();
        }

        /**
         * Test method: Seed the database with some fake user data
         */
        public static void SeedUserData()
		{
            Member.Collection.Insert(new Member {
                    Name = "Howard Price",
                    Street = "1373 Armistice Circle",
                    City = "Memphis",
                    State = "TN",
                    Zip = 38161
                    });
            Member.Collection.Insert(new Member {
                    Name = "Robin Smith",
                    Street = "94565 Sundown Ct",
                    City = "Las Vegas",
                    State = "NV",
                    Zip = 89105
                    });
            Member.Collection.Insert(new Member {
                    Name = "Donna Hernandez",
                    Street = "9 Nancy St",
                    City = "Dayton",
                    State = "OH",
                    Zip = 45414
                    });
            Provider.Collection.Insert(new Provider {
                    Name = "Phillip Parker",
                    Street = "982 Express Point",
                    City = "Austin",
                    State = "TX",
                    Zip = 78710
                    });
            Provider.Collection.Insert(new Provider {
                    Name = "Diana Brown",
                    Street = "2157 Moose Crossing",
                    City = "Muskegon",
                    State = "MI",
                    Zip = 49444
                    });
        }
    }
}

