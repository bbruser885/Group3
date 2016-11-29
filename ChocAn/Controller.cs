using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ChocAn
{

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
            var provider = View.ReadProviderById("Enter your provider ID number");
		    if (provider == null) return;
            var member = View.ReadMemberById();
		    if (member == null) return;
            var service = View.ReadServiceById();
		    if (service == null) return;
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
           Console.WriteLine("Please select a menu option");
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
        }

        private void CreateMember()
        {
            Console.WriteLine("Add a Member");
            var member = View.ReadMember();
            Console.WriteLine("New Member Information");
            View.PrintUser(member);
            Console.WriteLine();
            Console.WriteLine("Would you like to add this member?(Y/N)");
            String response = Console.ReadLine();
            Console.WriteLine();
            if (response == "y" || response == "Y")
            {
                Member.Collection.Insert(member);
                Console.WriteLine("The new member has been added to the database.");
            }
            else
            {
                Console.WriteLine("The member will not be added to the database");
            }

        }


        private void CreateProvider()
        {
            Console.WriteLine("Add a Provider");
            Provider provider = View.ReadProvider();
            Console.WriteLine("New Provider Information");
            View.PrintUser(provider);
            Console.WriteLine();
            Console.WriteLine("Would you like to add this Provider(Y/N)");
            String response = Console.ReadLine();
            Console.WriteLine();
            if (response == "y" || response == "Y")
            {
                Provider.Collection.Insert(provider);
                Console.WriteLine("The new Provider has been added to the database.");
            }
            else
            {
                Console.WriteLine("The user will not be added to the database");
            }

        }

        private void CreateManager()
        {
            Console.WriteLine("Add a Manager");
            Manager manager = View.ReadManager();
            Console.WriteLine("New Manager Information");
            View.PrintUser(manager);
            Console.WriteLine();
            Console.WriteLine("Would you like to add this Manager?(Y/N)");
            String response = Console.ReadLine();
            Console.WriteLine();
            if (response == "y" || response == "Y")
            {
                Manager.Collection.Insert(manager);
                Console.WriteLine("The new Manager has been added to the database.");
            }
            else
            {
                Console.WriteLine("The user will not be added to the database");
            }

        }

        public void EditUser()
        {
            var userType = View.UserTypeMenu();
            if (userType == null) return;
            BaseUser user = null;
            if (userType == typeof(Member))
            {
                user = View.ReadMemberById("Enter the ID of the member to edit");
            } else if (userType == typeof(Provider))
            {
                user = View.ReadProviderById("Enter the ID of the provider to edit");
            } else if (userType == typeof(Manager))
            {
                user = View.ReadManagerById("Enter the ID of the manager to edit");
            }
            if (user == null) return;
            View.ReadUpdatedPropertiesFor(ref user);
            View.PrintUser(user);
            var save = View.Confirm("Save this information?");
            if (save)
            {
                if (userType == typeof(Member))
                    Member.Collection.Update((Member) user);
                else if (userType == typeof(Provider))
                    Provider.Collection.Update((Provider) user);
                else if (userType == typeof(Manager))
                    Manager.Collection.Update((Manager) user);
                View.PrintSuccess("Record saved.");
            }
            else
            {
                View.PrintError("Abandoning Changes.");
            }

        }

        public void DeleteUser()
        {
            int choice;
            do
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("-- Delete User -----------------------------");
                Console.ResetColor();
                Console.WriteLine("1. Delete Member");
                Console.WriteLine("2. Delete Provider");
                Console.WriteLine("3. Delete Manger");
                Console.WriteLine("0. Cancel");
                choice = View.ReadInt("Your choice");
            } while (choice < 0 || choice > 4);
            switch (choice)
            {
                case 1:
                    DeleteMember();
                    break;
                case 2:
                    DeleteProvider();
                    break;
                case 3:
                    DeleteManger();
                    break;
                
            }
        }

        public void DeleteMember()
        {
            Member memberToDelete;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("-- Delete Member -----------------------------");
            Console.WriteLine();
            Console.ResetColor();
            memberToDelete = View.ReadMemberById();
            Console.WriteLine();
            View.PrintUser(memberToDelete);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Would you like to delete this Member?(Y/N)");
            Console.ResetColor();
            string choice = Console.ReadLine();
            while(choice!="y" && choice != "Y" && choice != "n" && choice != "N")
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Invalid Input");
                Console.WriteLine("Would you like to delete this Member?(Y/N)");
                Console.ResetColor();
                choice = Console.ReadLine();
            }
            switch (choice)
            {
                case "Y":
                case "y":
                    Member.Collection.Delete(memberToDelete.Id);
                    break;
                case "n":
                case "N":
                    DeleteUser();
                    break;
            }
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Member Deleted");
            Console.ResetColor();


        }

        public void DeleteProvider()
        {
            Provider providerToDelete;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("-- Delete Provider -----------------------------");
            Console.WriteLine();
            Console.ResetColor();
            providerToDelete = View.ReadProviderById();
            Console.WriteLine();
            View.PrintUser(providerToDelete);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Would you like to delete this Provider?(Y/N)");
            Console.ResetColor();
            string choice = Console.ReadLine();
            while (choice != "y" && choice != "Y" && choice != "n" && choice != "N")
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Invalid Input");
                Console.WriteLine("Would you like to delete this Provider?(Y/N)");
                Console.ResetColor();
                choice = Console.ReadLine();
            }
            switch (choice)
            {
                case "Y":
                case "y":
                    Provider.Collection.Delete(providerToDelete.Id);
                    break;
                case "n":
                case "N":
                    DeleteUser();
                    break;
            }
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Provider Deleted");
            Console.ResetColor();
        }

        public void DeleteManger()
        {
            Manager managerToDelete;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("-- Delete Manager -----------------------------");
            Console.WriteLine();
            Console.ResetColor();
            managerToDelete = View.ReadManagerById();
            Console.WriteLine();
            View.PrintUser(managerToDelete);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Would you like to delete this Manager?(Y/N)");
            Console.ResetColor();
            string choice = Console.ReadLine();
            while (choice != "y" && choice != "Y" && choice != "n" && choice != "N")
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Invalid Input");
                Console.WriteLine("Would you like to delete this Manager?(Y/N)");
                Console.ResetColor();
                choice = Console.ReadLine();
            }
            switch (choice)
            {
                case "Y":
                case "y":
                    Manager.Collection.Delete(managerToDelete.Id);
                    break;
                case "n":
                case "N":
                    DeleteUser();
                    break;
            }
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Manager Deleted");
            Console.ResetColor();
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
            Directory.CreateDirectory("Group3/ChocAn/consultationFiles/");
            File.WriteAllLines(@"Group3/ChocAn/consultationFiles/" + fileName , consultation);
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

