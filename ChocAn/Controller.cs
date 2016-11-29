using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace ChocAn
{
    public struct UserData
    {
        public String name;
        public String street;
        public String city;
        public String state;
        public String zip;
    }


    public sealed class Controller
    {
        private static readonly string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string ReportsDir = BaseDir + "reports";
        private const string DateFormat = "MM-dd-yyyy";

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
                    SeedUserData();
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
		
            //Create an array of strings holding the consultation to write to file
            string[] consultation = {member.ToString(),
				     provider.ToString(), 
				     service.ToString(), 
				     date.ToString()};
		    String addMember = member.Name;
            //Create a concultation object for the database collection and insert
            Consultation.Collection.Insert(new Consultation {
                ProviderRecord = provider,
                MemberRecord = member,
                ServiceRecord = service,
                Date = date
            });
            Console.WriteLine(member.ToString());
            Console.WriteLine(service.ToString());
            Console.WriteLine(date.Date.ToString("MMMM-dd-yyyy"));
            Console.WriteLine("Press any key to contiue");
		    Console.ReadKey();
            //Consultation was created, Write copy to file
            writeConsultationToFile(consultation, addMember);
        }

        public void RequestDirectory()
        {
            View.PrintError("Not implemented yet.");
        }

        public void RunMemberReport()
        {
            var start = DateTime.Now.Subtract(TimeSpan.FromDays(7));
            var end = DateTime.Now;

            var FriendlyDateFormat = $"dddd {DateFormat}";
            Console.WriteLine("Producing member reports for {0} through {1}",
                start.ToString(FriendlyDateFormat),
                end.ToString(FriendlyDateFormat)
            );

            var memberConsultations = Consultation.Collection.Find(c =>
                c.Date > start && c.Date < end
            ).OrderBy(
                c => c.MemberRecord.Name
            ).GroupBy(
                c => c.MemberRecord
            );

            var memberDirectory = ReportsDir + "\\member";
            Directory.CreateDirectory(memberDirectory);

            var total = 0;
            var memberConsultationCount = 1;
            foreach (var group in memberConsultations)
            {
                var member = group.Key;
                var output = new StringBuilder();
                output.Append(member);
                output.Append(Environment.NewLine);
                output.Append(string.Format(
                    "Services received during the week {0} through {1}:",
                    start.ToString(DateFormat),
                    end.ToString(DateFormat)
                ));
                foreach (var consultation in group)
                {
                    output.Append(Environment.NewLine);
                    output.Append($"  Date: {consultation.Date.ToString(DateFormat)}");
                    output.Append(Environment.NewLine);
                    output.Append($"  Provider: {consultation.ProviderRecord.Name}");
                    output.Append(Environment.NewLine);
                    output.Append($"  Name: {consultation.ServiceRecord.Name}");
                    output.Append(Environment.NewLine);
                }
                var filename = Regex.Replace(member.Name, @"\s+", "") +
                               end.ToString(DateFormat) +
                               ".txt";
                var path = $"{memberDirectory}/{filename}";
                File.WriteAllText(path, output.ToString());
                total++;
            }

            Console.WriteLine($"Wrote {total} member reports to {memberDirectory}");
        }

        public void RunProviderReport()
        {
            View.PrintError("Not implemented yet.");
        }

        public void RunAPReport()
        {
            View.PrintError("Not implemented yet.");
        }

        public void RunEFTReport()
        {
            View.PrintError("Not implemented yet.");
        }

        public void RunAllReports()
        {
            View.PrintError("Not implemented yet.");
        }

        public void CreateUser()
        {
           String control = "z";
           Console.WriteLine("Please Select a menu option");
           Console.WriteLine("1. Create Member");
           Console.WriteLine("2. Create Provider");
           Console.WriteLine("3. Create a Manager");

            while (control != "0")
            {
                control = Console.ReadLine();
                switch (control)
                {
                    case "1":
                        CreateMember();
                        control = "0";
                        break;
                    case "2":
                        CreateProvider();
                        control = "0";
                        break;
                    case "3":
                        CreateManager();
                        control = "0";
                        break;
                    case "0":
                        control = "0";
                        break;

                    default:
                        break;

                }

            }

            //View.PrintError("Not implemented yet.");
        }

        private void CreateMember()
        {
            Console.WriteLine("Add a Member");
            UserData info = BuildData();
            Console.WriteLine("New Member Information");
            printData(info);
            Console.WriteLine();
            Console.WriteLine("Would you like to add this member?(Y/N)");
            String response = Console.ReadLine();
            Console.WriteLine();
            if (response == "y" || response == "Y")
            {
                Console.WriteLine("The new Member has been added to the database.");
                Member.Collection.Insert(new Member
                {
                    Name = info.name,
                    Street = info.street,
                    City = info.city,
                    State = info.state,
                    Zip = Convert.ToInt32(info.zip),
                });
            }
            else
            {
                Console.WriteLine("The user will not be added to the database");
            }

        }


        private void CreateProvider()
        {
            Console.WriteLine("Add a Provider");
            UserData info = BuildData();
            Console.WriteLine("New Provider Information");
            printData(info);
            Console.WriteLine();
            Console.WriteLine("Would you like to add this Provider(Y/N)");
            String response = Console.ReadLine();
            Console.WriteLine();
            if (response == "y" || response == "Y")
            {
                Console.WriteLine("The new Provider has been added to the database.");
                Provider.Collection.Insert(new Provider()
                {
                    Name = info.name,
                    Street = info.street,
                    City = info.city,
                    State = info.state,
                    Zip = Convert.ToInt32(info.zip),
                });
            }
            else
            {
                Console.WriteLine("The user will not be added to the database");
            }

        }

        private void CreateManager()
        {
            Console.WriteLine("Add a Manager");
            UserData info = BuildData();
            Console.WriteLine("New Manager Information");
            printData(info);
            Console.WriteLine();
            Console.WriteLine("Would you like to add this Manager?(Y/N)");
            String response = Console.ReadLine();
            Console.WriteLine();
            if (response == "y" || response == "Y")
            {
                Console.WriteLine("The new Manager has been added to the database.");
                Manager.Collection.Insert(new Manager
                {
                    Name = info.name,
                    Street = info.street,
                    City = info.city,
                    State = info.state,
                    Zip = Convert.ToInt32(info.zip),
                });
            }
            else
            {
                Console.WriteLine("The user will not be added to the database");
            }

        }



        private UserData BuildData()
        {
            UserData info = new UserData();
            Console.Write("Full Name:");
            info.name = Console.ReadLine();
            Console.Write("Street:");
            info.street = Console.ReadLine();
            Console.Write("City:");
            info.city = Console.ReadLine();
            Console.Write("State:");
            info.state = Console.ReadLine();
            Console.Write("Zip:");
            info.zip = Console.ReadLine();
            return info;

        }

        private void printData(UserData info)
        {
            Console.WriteLine(info.name);
            Console.WriteLine(info.street);
            Console.WriteLine(info.city);
            Console.WriteLine(info.state);
            Console.WriteLine(info.zip);

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

        public void ClearManagerData()
        {
            BaseModel.ClearManagerData();
        }
	
        //=====================================================================
        //Function Name: GetTimestamp
        //Description: This function pull data from a DateTime object and
        //create a string that can be used for a file name.
        //Input: value (DateTime)
        //Output: string
        //Last Updated: 11.26.2016
        //=====================================================================
        public static String GetTimestamp(DateTime value)
        {
           // String pattern = "MM.dd.yyyy";

            return value.ToString(("MMMM.dd.yyyy") + ".");
        }    
	    
        //=========================================================================
        //Function Name: writeConsultationToFile
        //Description: This function will take an array of strings that hold the
        //the information for a consultation that has been created. This function
        //will take the data and write it to file in a prediscussed format.
        //The designated filepath will be: @Group3/ChocAn/consultationFiles/
        //Input: consultation (string[])
        //Output: none for now.
        //Last updated: 11.26.2016 16:09
        //=========================================================================
        public void writeConsultationToFile(string[] consultation, String name)
        {
            //create the file name for the consultation
            string currentTime = GetTimestamp(DateTime.Now);
            string fileName = "consultation." +
                name +
                currentTime +
                ".txt";

            //Create unique file for a consultation in the designated
            //file directory
            System.IO.Directory.CreateDirectory("Group3/ChocAn/consultationFiles/");
            System.IO.File.WriteAllLines(@"Group3/ChocAn/consultationFiles/" + fileName , consultation);
        }
	
        /**
         * Test method: Seed the database with some fake user data
         */
        public void SeedUserData()
        {
            Console.WriteLine("Please wait...");
            var memberSeeds = Regex.Split(Properties.Resources.SeedMembers, "\r\n|\r|\n");
            foreach (var line in memberSeeds)
            {
                var data = line.Split(',');
                if (data.Length == 5)
                {
                    Member.Collection.Insert(new Member
                    {
                        Name = data[0],
                        Street = data[1],
                        City = data[2],
                        State = data[3],
                        Zip = int.Parse(data[4])
                    });
                }
            }

            var providerSeeds = Regex.Split(Properties.Resources.SeedProviders, "\r\n|\r|\n");
            foreach (var line in providerSeeds)
            {
                if (line.Length == 0) break;
                var data = line.Split(',');
                if (data.Length == 5)
                {
                    Provider.Collection.Insert(new Provider
                    {
                        Name = data[0],
                        Street = data[1],
                        City = data[2],
                        State = data[3],
                        Zip = int.Parse(data[4])
                    });
                }
            }

            var managerSeeds = Regex.Split(Properties.Resources.SeedManagers, "\r\n|\r|\n");
            foreach (var line in managerSeeds)
            {
                if (line.Length == 0) break;
                var data = line.Split(',');
                if (data.Length == 5)
                {
                    Manager.Collection.Insert(new Manager
                    {
                        Name = data[0],
                        Street = data[1],
                        City = data[2],
                        State = data[3],
                        Zip = int.Parse(data[4])
                    });
                }
            }

            for (int i = 0; i < 25; i++)
            {
                // Randomize consultation dates to within the last month or so
                Random gen = new Random();
                DateTime date = DateTime.Now.Subtract(TimeSpan.FromDays(gen.Next(0,30)));
                Consultation.Collection.Insert(new Consultation
                {
                    MemberRecord = Member.Collection.FindById(gen.Next(1, 100)),
                    ProviderRecord = Provider.Collection.FindById(gen.Next(1, 50)),
                    ServiceRecord = Service.Collection.FindById(gen.Next(1, 8)),
                    Date = date
                });
            }
            Console.WriteLine("100 Members, 50 Providers, 25 Managers, and 25 recent consultations have been created.");
        }
        
        public IEnumerable<BaseModel> getManagers()
        {
            var col = Manager.Collection.FindAll();
            
            
            return col;
        }
        public IEnumerable<BaseModel> getProviders()
        {
            var col = Provider.Collection.FindAll();


            return col;
        }
        public IEnumerable<BaseModel> getMembers()
        {
            var col = Member.Collection.FindAll();


            return col;
        }

    }
   
}

