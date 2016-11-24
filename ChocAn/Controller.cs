using System;

namespace ChocAn
{
    public sealed class Controller
    {
        private static readonly Controller SingleInstance = new Controller();

        private static readonly View View = new View();

        static Controller() { }

        public static Controller Instance => SingleInstance;

        private BaseUser _currentUser = null;

        public BaseUser GetCurrentUser() => _currentUser;

        public void Run()
        {
            while (_currentUser == null)
            {
                var loginOption = View.LoginMenu();

                if (loginOption == LogInOptions.Exit)
                {
                    Environment.Exit(0);
                }

                if (loginOption == LogInOptions.SeedData)
                {
                    SingleInstance.SeedUserData();
                }
                else
                {
                    var userId = View.ReadInt($"Enter your {loginOption.GetString()} ID to log in");
                    _currentUser = loginOption == LogInOptions.Manager ?
                        Manager.Collection.FindById(userId) :
                        Provider.Collection.FindById(userId);

                    if (_currentUser == null)
                    {
                        View.PrintError($"Invalid {loginOption.GetString()} ID.");
                    }
                    else
                    {
                        View.MainMenu();
                        _currentUser = null;
                    }
                }
            }
        }

        public void CreateConsultation()
		{
            var provider = View.ReadProviderById();
            var member = View.ReadMemberById();
            var service = View.ReadServiceById();
            var date = View.ReadDateTime("Date of consultation");
            Consultation.Collection.Insert(new Consultation {
                ProviderRecord = provider,
                MemberRecord = member,
                ServiceRecord = service,
                Date = date
            });
        }

        public void RequestDirectory()
        {
            View.PrintError("Not implemented yet.");
        }

        public void RunReport()
        {
            View.PrintError("Not implemented yet.");
        }

        public void RunAllReports()
        {
            View.PrintError("Not implemented yet.");
        }

        public void CreateUser()
        {
            View.PrintError("Not implemented yet.");
        }

        public void EditUser()
        {
            View.PrintError("Not implemented yet.");
        }

        public void DeleteUser()
        {
            View.PrintError("Not implemented yet.");
        }

        public void ClearUserData()
        {
            BaseModel.ClearUserData();
        }

        /**
         * Test method: Seed the database with some fake user data
         */
        public void SeedUserData()
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

