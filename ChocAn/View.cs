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
            new KeyValuePair<string, Action>("Run a report", Controller.RunReport),
            new KeyValuePair<string, Action>("Run all reports", Controller.RunAllReports),
            new KeyValuePair<string, Action>("Create a new user", Controller.CreateUser),
            new KeyValuePair<string, Action>("Edit a user record", Controller.EditUser),
            new KeyValuePair<string, Action>("Delete a user record", Controller.DeleteUser),
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
            Console.ForegroundColor = ConsoleColor.Blue;
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

        public static void PrintPrompt(string prompt)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{prompt}: ");
            Console.ResetColor();
        }

        public LogInOptions LoginMenu()
        {
            int choice;
            do
            {
                Console.ForegroundColor = ConsoleColor.Blue;
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
                    !Enumerable.Range(1, menuOptions.Count).Contains(choice) ) continue;
                menuOptions[choice-1].Value();
                Console.WriteLine();

            } while (choice != 0);
            Console.WriteLine();
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
                PrintPrompt($"{propertyName}: ");
                value = Console.ReadLine();
                // Validate the property
                var results = new List<ValidationResult>();
                valid = Validator.TryValidateProperty(value, context, results);
                foreach (var result in results)
                {
                    // Print the error messages from any failed validations for the user
                    Console.WriteLine(result);
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
                Console.Write(string.Format("{0}: ", propertyName));
                value = Convert.ToInt32(Console.ReadLine());
                // Validate the property
                var results = new List<ValidationResult>();
                valid = Validator.TryValidateProperty(value, context, results);
                foreach (var result in results)
                {
                    // Print the error messages from any failed validations for the user
                    Console.WriteLine(result);
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

        public Provider ReadProviderById()
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
                }
            } while (provider == null);
            return provider;
        }

        public Member ReadMemberById()
        {
            Member member = null;
            do
            {
                PrintPrompt("Enter the member ID number");
                int id;
                if (!int.TryParse(Console.ReadLine(), out id)) continue;
                member = Member.Collection.FindById(id);
                if (member == null)
                {
                    PrintError("Couldn't find a member with that ID number.");
                }
            } while (member == null);
            return member;
        }

        public Manager ReadManagerById()
        {
            Manager manager = null;
            do
            {
                PrintPrompt("Enter the manager ID number");
                int id;
                if (!int.TryParse(Console.ReadLine(), out id)) continue;
                manager = Manager.Collection.FindById(id);
                if (manager == null)
                {
                    PrintError("Couldn't find a manager with that ID number");
                }
            } while (manager == null);
            return manager;
        }

        public Service ReadServiceById()
        {
            Service service = null;
            do
            {
                PrintPrompt("Enter the service ID number");
                int id;
                if (!int.TryParse(Console.ReadLine(), out id)) continue;
                service = Service.Collection.FindById(id);
                if (service == null)
                {
                    PrintError("Couldn't find a service with that ID.");
                }
            } while (service == null);
            return service;
        }

        public DateTime ReadDateTime(string prompt = "Enter a date")
        {
            string dateString;
            DateTime date;
            do
            {
                PrintPrompt(prompt + " (MM-DD-YYYY): ");
                dateString = Console.ReadLine();
            } while (!DateTime.TryParseExact(dateString, "MM-dd-yyyy",
                        EnUs, DateTimeStyles.None, out date));
            return date;
        }


        public int ReadInt(string prompt = "Enter a number")
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

        /**
         * Calls the print function on some set of models, e.g. a resultset
         * produced by Collection.FindAll()
         */
        public void PrintAll(IEnumerable<BaseModel> items)
        {
            foreach (var item in items)
            {
                item.Print();
            }
        }

        private void DumpDB(IEnumerable<BaseModel> items)
        {
            StringBuilder text = new StringBuilder();

            foreach (var item in items)
            {
                text.Append(item.ToString());
            }
        }
    }
}

