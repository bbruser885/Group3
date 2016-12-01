using System;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ChocAn
{
    public sealed class View
    {
        private static readonly Controller Controller = Controller.Instance;

        private static readonly CultureInfo EnUs = new CultureInfo("en-US");

        private readonly List<KeyValuePair<string, Action>> _providerMenuOptions = new List<KeyValuePair<string, Action>>
        {
            new KeyValuePair<string, Action>("Enter a consultation", Controller.Instance.CreateConsultation),
            new KeyValuePair<string, Action>("Request a copy of the Provider Directory", Controller.Instance.RequestDirectory),
        };

        private readonly List<KeyValuePair<string, Action>> _managerMenuOptions = new List<KeyValuePair<string, Action>>
        {
            new KeyValuePair<string, Action>("Run a report", ReportMenu),
            new KeyValuePair<string, Action>("Run all reports", Controller.RunAllReports),
            new KeyValuePair<string, Action>("Create a new user", Controller.CreateUser),
            new KeyValuePair<string, Action>("Edit a user record", Controller.EditUser),
            new KeyValuePair<string, Action>("Delete a user record", Controller.DeleteUser),
            new KeyValuePair<string, Action>("View a user record", Controller.ViewUser),
            new KeyValuePair<string, Action>("Dump Database(Debug)", DumpDBWrapper),
            new KeyValuePair<string, Action>("Delete Database(Debug)", DeleteDB),
        };

        private List<KeyValuePair<string, Action>> GetMenuOptions()
        {
            if (Controller.GetCurrentUser().GetType() == typeof(Manager))
            {
                return _providerMenuOptions.Concat(_managerMenuOptions).ToList();
            }
            else
            {
                return _providerMenuOptions;
            }
        }

        private void PrintMenu(IReadOnlyList<KeyValuePair<string, Action>> menuOptions)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("-- ChocAn Menu ------------------------------");
            Console.ResetColor();

            for (var i = 0; i < menuOptions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {menuOptions[i].Key}");
            }
            Console.WriteLine("0. Log out");
        }

        public static void PrintError(string message = "Error")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void PrintSuccess(string message = "Success")
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public static void PrintPrompt(string prompt)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{prompt}: ");
            Console.ResetColor();
        }

        public LogInOptions LoginMenu()
        {
            int choice;
            do
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("-- ChocAn Login -----------------------------");
                Console.ResetColor();
                Console.WriteLine("1. Log in as a provider");
                Console.WriteLine("2. Log in as a manager");
                Console.WriteLine("3. Seed Data");
                Console.WriteLine("0. Quit");
                choice = ReadInt("Your choice");
            } while (choice < 0 || choice > 3);
            switch (choice)
            {
                case 1:
                    return LogInOptions.Provider;
                case 2:
                    return LogInOptions.Manager;
                case 3:
                    return LogInOptions.SeedData;
                default:
                    return LogInOptions.Exit;
            }
        }

        public void MainMenu()
        {
            int choice;
            Console.WriteLine();
            do
            {
                var menuOptions = GetMenuOptions();
                PrintMenu(menuOptions);
                PrintPrompt("Your choice");
                var input = Console.ReadLine();
                if (!int.TryParse(input, out choice) ||
                    !Enumerable.Range(1, menuOptions.Count).Contains(choice)) continue;
                menuOptions[choice-1].Value();
                Console.WriteLine();

            } while (choice != 0);
            Console.WriteLine();
        }

        public static void ReportMenu()
        {
            int choice;
            do
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("-- Run a Report -----------------------------");
                Console.ResetColor();
                Console.WriteLine("1. Member Report");
                Console.WriteLine("2. Provider Report");
                Console.WriteLine("3. AP Report");
                Console.WriteLine("4. EFT Report");
                Console.WriteLine("0. Cancel");
                choice = ReadInt("Your choice");
            } while (choice < 0 || choice > 4);
            switch (choice)
            {
                case 1:
                    Controller.RunMemberReport();
                    break;
                case 2:
                    Controller.RunProviderReport();
                    break;
                case 3:
                    Controller.RunAPReport();
                    break;
                case 4:
                    Controller.RunEFTReport();
                    break;
            }
        }

        public bool Confirm(string prompt = "Try again?")
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(prompt + " (Y/n) ");
            Console.ResetColor();
            var response = Console.ReadLine();
            if (response.StartsWith("N") || response.StartsWith("n")) return false;
            return true;
        }

        public Type UserTypeMenu(string prompt = "Your choice")
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Which user type?");
            Console.ResetColor();
            Console.WriteLine("1. Member");
            Console.WriteLine("2. Provider");
            Console.WriteLine("3. Manager");
            Console.WriteLine("0. Cancel");
            var choice = ReadInt(prompt);
            switch (choice)
            {
                case 1:
                    return typeof(Member);
                case 2:
                    return typeof(Provider);
                case 3:
                    return typeof(Manager);
                default:
                    return null;
            }
        }

        public void ReadUpdatedPropertiesFor(ref BaseUser user)
        {
            var maxChoice = 5;
            var choice = -1;
            do
            {
                Console.WriteLine("Current data:");
                Console.Write(user.ToString());
                Console.WriteLine("-------------");
                Console.WriteLine("Change which field?");
                Console.WriteLine("1. Name");
                Console.WriteLine("2. Street");
                Console.WriteLine("3. City");
                Console.WriteLine("4. State");
                Console.WriteLine("5. Zip");
                if (user.GetType() == typeof(Member))
                {
                    Console.WriteLine("6. Suspension Status");
                    maxChoice = 6;
                }
                Console.WriteLine("0. (Done)");
                choice = ReadInt("Your choice");
                switch (choice)
                {
                    case 1:
                        var name = ReadValidStringFor(user, "Name");
                        user.Name = name;
                        break;
                    case 2:
                        var street = ReadValidStringFor(user, "Street");
                        user.Street = street;
                        break;
                    case 3:
                        var city = ReadValidStringFor(user, "City");
                        user.City = city;
                        break;
                    case 4:
                        var state = ReadValidStringFor(user, "State");
                        user.State = state;
                        break;
                    case 5:
                        var zip = ReadValidIntFor(user, "Zip");
                        user.Zip = zip;
                        break;
                    case 6:
                        ((Member) user).Suspended = !((Member)user).Suspended;
                        break;
                }
            } while (choice > 0 && choice < maxChoice + 1);
        }

        /**
         * Use the validation annotations on a model to get valid inputs from
         * the user for a string field.
         * (See also MS's docs for System.ComponentModel.DataAnnotations)
         *
         * Params:
         *    modelInstance: an instance of a class (Member, Provider, etc.)
         *    propertyName: a string with the name of the property you want to set
         *
         * Returns: a valid string value for the property you passed in
         *
         * Example:
         *    Member m = new Member();
         *    m.Name = ReadValidStringFor(m, "Name");
         *    // m now has a Name string meeting any validation requirements
         */
        private static string ReadValidStringFor(object modelInstance, string propertyName)
        {
            string value;
            bool valid = false;
            var context = new ValidationContext(modelInstance) { MemberName = propertyName };
            do
            {
                // Use the property's name to prompt the user
                PrintPrompt(propertyName);
                value = Console.ReadLine();
                // Validate the property
                var results = new List<ValidationResult>();
                valid = Validator.TryValidateProperty(value, context, results);
                foreach (var result in results)
                {
                    // Print the error messages from any failed validations for the user
                    PrintError(result.ToString());
                }
            } while (!valid);
            return value;
        }

        /**
         * Use the validation annotations on a model to get valid inputs from
         * the user for an integer field.
         * (See also MS's docs for System.ComponentModel.DataAnnotations)
         *
         * Params:
         *    modelInstance: an instance of a class (Member, Provider, etc.)
         *    propertyName: a string with the name of the property you want to set
         *
         * Returns: a valid integer value for the property you passed in
         *
         * Example:
         *    Member m = new Member();
         *    m.Name = ReadValidIntFor(m, "Age");
         *    // m now has an Age integer meeting any validation requirements
         */
        private static int ReadValidIntFor(object modelInstance, string propertyName)
        {
            int value;
            bool valid = false;
            var context = new ValidationContext(modelInstance) { MemberName = propertyName };
            do
            {
                // Use the property's name to prompt the user
                PrintPrompt(propertyName);
                // Read an int
                if (!int.TryParse(Console.ReadLine(), out value))
                {
                    PrintError("You must enter a number.");
                    continue;
                }
                // Validate the property
                var results = new List<ValidationResult>();
                valid = Validator.TryValidateProperty(value, context, results);
                foreach (var result in results)
                {
                    // Print the error messages from any failed validations for the user
                    PrintError(result.ToString());
                }
            } while (!valid);
            return value;
        }


        /**
         * Prompts for and fills in common data for any user classes.
         *
         * Params:
         * type: One of the user models from the UserTypes enum above.
         *       Determines which type of model will be returned.
         *
         * Returns: an initialized instance of the type you passed in
         */
        private static BaseUser ReadUser(BaseUser user) {
            Console.WriteLine();

            user.Name = ReadValidStringFor(user, "Name");
            user.Street = ReadValidStringFor(user, "Street");
            user.City = ReadValidStringFor(user, "City");
            user.State = ReadValidStringFor(user, "State");
            user.Zip = ReadValidIntFor(user, "Zip");

            return user;
        }

        public Member ReadMember()
        {
            return (Member) ReadUser(new Member());
        }

        public Provider ReadProvider()
        {
            return (Provider) ReadUser(new Provider());
        }

        public Manager ReadManager()
        {
            return (Manager) ReadUser(new Manager());
        }

        public Provider ReadProviderById(string prompt = "Enter the provider ID number")
        {
            Provider provider = null;
            do
            {
                PrintPrompt("Enter your provider ID number");
                int id;
                if (!int.TryParse(Console.ReadLine(), out id)) continue;
                provider = Provider.Collection.FindById(id);
                if (provider == null)
                {
                    Console.WriteLine("Couldn't find a provider with that ID number.");
                    if (!Confirm()) return null;
                }
            } while (provider == null);
            return provider;
        }

        public Member ReadMemberById(string prompt = "Enter the member ID number")
        {
            Member member = null;
            do
            {
                PrintPrompt(prompt);
                int id;
                if (!int.TryParse(Console.ReadLine(), out id)) continue;
                member = Member.Collection.FindById(id);
                if (member == null)
                {
                    PrintError("Couldn't find a member with that ID number.");
                    if (!Confirm()) return null;
                }
            } while (member == null);
            return member;
        }

        public Manager ReadManagerById(string prompt = "Enter the manager ID number")
        {
            Manager manager = null;
            do
            {
                PrintPrompt(prompt);
                int id;
                if (!int.TryParse(Console.ReadLine(), out id)) continue;
                manager = Manager.Collection.FindById(id);
                if (manager == null)
                {
                    PrintError("Couldn't find a manager with that ID number");
                    if (!Confirm()) return null;
                }
            } while (manager == null);
            return manager;
        }

        public Service ReadServiceById(string prompt = "Enter the service ID number")
        {
            Service service = null;
            do
            {
                PrintPrompt(prompt);
                int id;
                if (!int.TryParse(Console.ReadLine(), out id)) continue;
                service = Service.Collection.FindById(id);
                if (service == null)
                {
                    PrintError("Couldn't find a service with that ID.");
                    if (!Confirm()) return null;
                }
            } while (service == null);
            return service;
        }

        public DateTime ReadDateTime(string prompt = "Enter a date")
        {
            string dateString;
            DateTime date;
            var defaultDate = DateTime.Now;
            do
            {
                PrintPrompt($"{prompt} ({defaultDate:MM-dd-yyyy})");
                dateString = Console.ReadLine();
                if (dateString.Length == 0) dateString = defaultDate.ToString("MM-dd-yyyy");
            } while (!DateTime.TryParseExact(dateString, "MM-dd-yyyy",
                        EnUs, DateTimeStyles.None, out date));
            return date;
        }

        public static int ReadInt(string prompt = "Enter a number")
        {
            int id;
            string input;
            do
            {
                PrintPrompt(prompt);
                input = Console.ReadLine();
            } while (!int.TryParse(input, out id));
            return id;
        }

        /*
         * Func:    ConvertToUserID
         * Purpose: converts database ID to User ID
         * Param:   int - IdToConvert
         * return:  string - 9 digit string for User ID
         * Revised: 12/1/16
         */
        public string ConvertToUserID(int IdToConvert)
        {
            return IdToConvert.ToString("D9");
        }

        public void PrintUser(BaseUser user)
        {
            Console.WriteLine($"Name: {user.Name}");
            Console.WriteLine($"ID: {ConvertToUserID(user.Id)}");
            Console.WriteLine("Address:");
            Console.WriteLine($"  Street: {user.Street}");
            Console.WriteLine($"  City: {user.City}");
            Console.WriteLine($"  State: {user.State}");
            Console.WriteLine($"  Zip: {user.Zip}");
            if (user.GetType() == typeof(Member))
            {
                var suspended = ((Member)user).Suspended ? "yes" : "no";
                Console.WriteLine($"Suspended: {suspended}");
            }
        }

        private static void DumpDBWrapper()
        {
            System.IO.File.Delete("databaseDump.txt");
            DumpDB(Controller.getManagers());
            DumpDB(Controller.getProviders());
            DumpDB(Controller.getMembers());
        }
        private static void DumpDB(IEnumerable<BaseModel> items)
        {
            StringBuilder text = new StringBuilder();

            foreach (var item in items)
            {
                text.Append(item.ToString());
            }
            System.IO.File.AppendAllText("databaseDump.txt", text.ToString());
        }
        
        //Test Method: Delete all UserData in database
        private static void DeleteDB()
        {
            Controller.ClearManagerData();
            Controller.ClearUserData();
        }

    }
}

