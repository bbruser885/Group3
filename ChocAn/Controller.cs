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
        private static readonly string ConsultationsDir = BaseDir + "consultations";
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
                        View.PrintSuccess($"Logged in as {_currentUser.Name}, ID {_currentUser.Id:D9}");
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
            var comments = View.ReadString("Comments", 100);

            var consultation = new Consultation
            {
                ProviderRecord = provider,
                MemberRecord = member,
                ServiceRecord = service,
                Date = date,
                Comments = comments
            };

            Console.WriteLine(consultation);
            if (!View.Confirm("Is this information correct?"))
            {
                View.PrintError("Cancelling consultation entry.");
                return;
            }
            Consultation.Collection.Insert(consultation);
            //Consultation was created, Write copy to file
            writeConsultationToFile(consultation);
            View.PrintSuccess("The following consultation has been saved to the ChocAn database.");
            Console.WriteLine(consultation);
            Console.WriteLine("Press any key to contiue");
            Console.ReadKey();
        }

        public void RequestDirectory()
        {
            int serviceTotal = Service.Collection.Count();
                var serviceDirectory = Service.Collection.FindAll().OrderBy(x => x.Name);
            StringBuilder text = new StringBuilder();

            foreach (var item in serviceDirectory)
            {
                text.Append(item.ToString());
            }
            var filename = "DirectoryList.txt";
            File.AppendAllText(filename, text.ToString());
            var path = BaseDir + filename;
            View.PrintSuccess($"Wrote {serviceTotal} service(s) to {path}");
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

            var memberDirectory = ReportsDir + Path.DirectorySeparatorChar + "member";
            Directory.CreateDirectory(memberDirectory);
            Console.WriteLine(memberDirectory);

            var total = 0;
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
                var path = memberDirectory + Path.DirectorySeparatorChar + filename;
                File.WriteAllText(path, output.ToString());
                total++;
            }

            View.PrintSuccess($"Wrote {total} member reports to {memberDirectory}");
        }

        public void RunProviderReport()
        {
            var start = DateTime.Now.Subtract(TimeSpan.FromDays(7));
            var end = DateTime.Now;

            var FriendlyDateFormat = $"dddd {DateFormat}";
            Console.WriteLine("Producing Provider reports for {0} through {1}",
                start.ToString(FriendlyDateFormat),
                end.ToString(FriendlyDateFormat)
            );

            var providerConsultations = Consultation.Collection.Find(c =>
                        c.Date > start && c.Date < end
            ).OrderBy(
                c => c.ProviderRecord.Name
            ).GroupBy(
                c => c.ProviderRecord
            );

            var providerdirectory = ReportsDir + Path.DirectorySeparatorChar + "provider";
            Directory.CreateDirectory(providerdirectory);

            var total = 0;
            var providerConsultationCount = 1;
            foreach (var group in providerConsultations)
            {
                var provider = group.Key;
                var output = new StringBuilder();
                output.Append(provider);
                output.Append(Environment.NewLine);
                output.Append(string.Format(
                    "Services provided during the week {0} through {1}:",
                    start.ToString(DateFormat),
                    end.ToString(DateFormat)
                ));
                foreach (var consultation in group)
                {
                    output.Append(Environment.NewLine);
                    output.Append($"  Date: {consultation.Date.ToString(DateFormat)}");
                    output.Append(Environment.NewLine);
                    output.Append($"  Member: {consultation.MemberRecord.Name}");
                    output.Append(Environment.NewLine);
                    output.Append($"  Name: {consultation.ServiceRecord.Name}");
                    output.Append(Environment.NewLine);
                    output.Append($"  Fee: {consultation.ServiceRecord.Fee:C}");
                    output.Append(Environment.NewLine);
                }
                var filename = Regex.Replace(provider.Name, @"\s+", "") +
                               end.ToString(DateFormat) +
                               ".txt";
                var path = providerdirectory + Path.DirectorySeparatorChar + filename;
                File.WriteAllText(path, output.ToString());
                total++;
            }

            View.PrintSuccess($"Wrote {total} provider reports to {providerdirectory}");
        }

        public void RunAPReport()
        {
            var totalProvider = 0;
            var totalFees = 0.0;
            var sumProvider = 0;
            var sumFees = 0.0;
            var start = DateTime.Now.Subtract(TimeSpan.FromDays(7));
            var end = DateTime.Now;

            var FriendlyDateFormat = $"dddd {DateFormat}";
            Console.WriteLine("Producing AP report for {0} through {1}",
                start.ToString(FriendlyDateFormat),
                end.ToString(FriendlyDateFormat)
            );

            var providerConsultations = Consultation.Collection.Find(c =>
                       c.Date > start && c.Date < end
           ).OrderBy(
               c => c.ProviderRecord.Name
           ).GroupBy(
               c => c.ProviderRecord
           );

            var APdirectory = ReportsDir + Path.DirectorySeparatorChar + "APSummary";
            Directory.CreateDirectory(APdirectory);
            var fileName = "APSummary" + end.ToString(DateFormat) + ".txt";
            var path = APdirectory + Path.DirectorySeparatorChar + fileName;
            foreach (var group in providerConsultations)
            {
                var provider = group.Key;
                var output = new StringBuilder();
                output.Append(provider);
                output.Append(Environment.NewLine);
                output.Append(string.Format(
                    "Consultations and Fees provided during the week {0} through {1}:",
                    start.ToString(DateFormat),
                    end.ToString(DateFormat)
                ));
                foreach (var consultation in group)
                {
                    totalProvider = totalProvider + 1;
                    totalFees = totalFees + consultation.ServiceRecord.Fee;
                }
                output.Append($"  Number of Consultations: {totalProvider}");
                output.Append(Environment.NewLine);
                output.Append($"  Total Fees: {totalFees:C}");
                output.Append(Environment.NewLine);
                output.Append(Environment.NewLine);

                
                File.AppendAllText(path, output.ToString());
                sumProvider = sumProvider + 1;
                sumFees = sumFees + totalFees;
                totalProvider = 0;
                totalFees = 0.0;
            }

            File.AppendAllText(path, "Total Number of Providers: ");
            File.AppendAllText(path, sumProvider.ToString());
            File.AppendAllText(path, Environment.NewLine);
            File.AppendAllText(path, "Total sum of Fees: ");
            File.AppendAllText(path, sumFees.ToString("C"));
            File.AppendAllText(path, Environment.NewLine);
            View.PrintSuccess($"Wrote AP report to {path}");
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
            var save = View.Confirm("Would you like to save this member?");
            if (save)
            {
                Member.Collection.Insert(member);
                View.PrintSuccess($"The new member has been added to the database with ID {member.Id}.");
            }
            else
            {
                View.PrintError("The member will not be added to the database");
            }
        }


        private void CreateProvider()
        {
            Console.WriteLine("Add a Provider");
            var provider = View.ReadProvider();
            Console.WriteLine("New Provider Information");
            View.PrintUser(provider);
            var save = View.Confirm("Would you like to save this provider?");
            if (save)
            {
                Provider.Collection.Insert(provider);
                View.PrintSuccess($"The new Provider has been added to the database with ID {provider.Id}.");
            }
            else
            {
                View.PrintError("The user will not be added to the database");
            }
        }

        private void CreateManager()
        {
            Console.WriteLine("Add a Manager");
            Manager manager = View.ReadManager();
            Console.WriteLine("New Manager Information");
            View.PrintUser(manager);
            var save = View.Confirm("Would you like to save this manager?");
            if (save)
            {
                Manager.Collection.Insert(manager);
                View.PrintSuccess($"The new manager has been added to the database with ID {manager.Id}.");
            }
            else
            {
                View.PrintError("The user will not be added to the database");
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
            }
            else if (userType == typeof(Provider))
            {
                user = View.ReadProviderById("Enter the ID of the provider to edit");
            }
            else if (userType == typeof(Manager))
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

        /*
         * Func:    DeleteUser
         * Purpose: allows manager to deletes user(member/provider/manager) 
         *          from the database
         * return:  void
         * Revised: 11/29/16
         */
        public void DeleteUser()
        {
            int choice;
            do
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine();
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

        /*
         * Func:    DeleteMember
         * Purpose: prompts manager for member ID, verifies member exits,
         *          deletes member after confimation
         * return:  void
         * Revised: 11/29/16
         */
        public void DeleteMember()
        {
            Member memberToDelete;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("-- Delete Member -----------------------------");
            Console.WriteLine();
            Console.ResetColor();
            memberToDelete = View.ReadMemberById();
            Console.WriteLine();
            View.PrintUser(memberToDelete);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Would you like to delete this Member?(Y/N)");
            Console.ResetColor();
            string choice = Console.ReadLine();
            while(choice!="y" && choice != "Y" && choice != "n" && choice != "N")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Member Deleted");
            Console.ResetColor();
        }

        /*
         * Func:    DeleteProvider
         * Purpose: prompts manager for provider ID, verifies provider exits,
         *          deletes provider after confimation
         * return:  void
         * Revised: 11/29/16
         */
        public void DeleteProvider()
        {
            Provider providerToDelete;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("-- Delete Provider -----------------------------");
            Console.WriteLine();
            Console.ResetColor();
            providerToDelete = View.ReadProviderById();
            Console.WriteLine();
            View.PrintUser(providerToDelete);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Would you like to delete this Provider?(Y/N)");
            Console.ResetColor();
            string choice = Console.ReadLine();
            while (choice != "y" && choice != "Y" && choice != "n" && choice != "N")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Provider Deleted");
            Console.ResetColor();
        }

        /*
         * Func:    DeleteManger
         * Purpose: prompts manager for manager to deletes ID, verifies
         *          manager exits, deletes manager after confimation
         * return:  void
         * Revised: 11/29/16
         */
        public void DeleteManger()
        {
            Manager managerToDelete;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("-- Delete Manager -----------------------------");
            Console.WriteLine();
            Console.ResetColor();
            managerToDelete = View.ReadManagerById();
            Console.WriteLine();
            View.PrintUser(managerToDelete);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Would you like to delete this Manager?(Y/N)");
            Console.ResetColor();
            string choice = Console.ReadLine();
            while (choice != "y" && choice != "Y" && choice != "n" && choice != "N")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Manager Deleted");
            Console.ResetColor();
        }

        /*Deletes all provider and member data from  database*/
        public void ClearUserData()
        {
            BaseModel.ClearUserData();
        }

        /*Deletes all manager data from database*/
        public void ClearManagerData()
        {
            BaseModel.ClearManagerData();
        }

        //=========================================================================
        //Function Name: writeConsultationToFile
        //Description: This function will take a newly saved consultation model.
        //It will write the data to a file.
        //The output directory will be: (application directory)/consultations/
        //Input: consultation (Consultation)
        //Output: none for now.
        //Last updated: 11.26.2016 16:09
        //=========================================================================
        public void writeConsultationToFile(Consultation consultation)
        {
            //create the file name for the consultation
            string fileName = "consultation." +
                              Regex.Replace(consultation.MemberRecord.Name, @"\s+", "") +
                              consultation.Date.ToString(DateFormat) +
                              ".txt";

            //Create unique file for a consultation in the designated
            //file directory
            Directory.CreateDirectory(ConsultationsDir);
            var path = ConsultationsDir + Path.DirectorySeparatorChar + fileName;
            File.WriteAllText(path, consultation.ToString());
            View.PrintSuccess($"Wrote consultation file to {path}.");
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
            View.PrintSuccess("100 Members, 50 Providers, 25 Managers, and 25 recent consultations have been created.");
        }

        public void ViewUser()
        {
            Type user = View.UserTypeMenu();
            BaseUser found = null;
            if (user == null)
            {
                View.PrintError("no user type selected");
                return;
            }
            else
            {
                if (user == typeof(Member))
                {
                    found = View.ReadMemberById();
                }
                else if (user == typeof(Provider))
                    found = View.ReadProviderById();
                else if (user == typeof(Manager))
                {
                    found = View.ReadManagerById();
                }
                if (found == null)
                {
                    View.PrintError("User not found");
                    return;
                }
                Console.WriteLine();
                View.PrintUser(found);
                Console.WriteLine();
                Console.WriteLine("Press any key to contiue");
                Console.ReadKey();
            }
        }
        
        public IEnumerable<BaseModel> getManagers()
        {
            return Manager.Collection.FindAll();
        }
        public IEnumerable<BaseModel> getProviders()
        {
            return Provider.Collection.FindAll();
        }
        public IEnumerable<BaseModel> getMembers()
        {
            return Member.Collection.FindAll();
        }

    }
   
}

