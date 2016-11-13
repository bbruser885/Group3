using System;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChocAn
{
    public class View
    {
        private static readonly CultureInfo EnUs = new CultureInfo("en-US");

        private static readonly string[] MenuOptions = {
            "Display all members",
            "Display all providers",
            "Display all managers",
            "Display all services",
            "Display all consultations",
            "Add a member",
            "Add a provider",
            "Add a manager",
            "Create a consultation",
            "Empty all user data",
            "Seed the database with some fake user data"
        };

        private static void PrintMenuOptions()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("-- ChocAn Menu ------------------------------");
            Console.ResetColor();
            for (int i = 0; i < MenuOptions.Length; i++)
            {
                Console.WriteLine(string.Format("{0}. {1}", i+1, MenuOptions[i]));
            }
            Console.WriteLine("0. Quit");
        }

        public static void MainMenuLoop()
        {
            int choice;
            do
            {
                PrintMenuOptions();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("Your choice: ");
                Console.ResetColor();
                var input = Console.ReadLine();
                // Prompt again if the input wasn't an integer
                if (!int.TryParse(input, out choice)) continue;
                switch (choice)
                {
                    case 1:
                        Controller.PrintAllMembers();
                        break;
                    case 2:
                        Controller.PrintAllProviders();
                        break;
                    case 3:
                        Controller.PrintAllManagers();
                        break;
                    case 4:
                        Controller.PrintAllServices();
                        break;
                    case 5:
                        Controller.PrintAllConsultations();
                        break;
                    case 6:
                        Controller.CreateMember();
                        break;
                    case 7:
                        Controller.CreateProvider();
                        break;
                    case 8:
                        Controller.CreateManager();
                        break;
                    case 9:
                        Controller.CreateConsultation();
                        break;
                    case 10:
                        Controller.ClearUserData();
                        break;
                    case 11:
                        Controller.SeedUserData();
                        break;
                    default:
                        break;
                }
                System.Console.WriteLine();

            } while (choice != 0);
        }

        public static void PrintError(string message = "Error")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
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
        protected static string ReadValidStringFor(object modelInstance, string propertyName)
        {
            string value;
            bool valid = false;
            var context = new ValidationContext(modelInstance) { MemberName = propertyName };
            do
            {
                // Use the property's name to prompt the user
                Console.Write(string.Format("{0}: ", propertyName));
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
        protected static int ReadValidIntFor(object modelInstance, string propertyName)
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
        protected static BaseUser ReadUser(BaseUser user) {
            Console.WriteLine();

            user.Name = ReadValidStringFor(user, "Name");
            user.Street = ReadValidStringFor(user, "Street");
            user.City = ReadValidStringFor(user, "City");
            user.State = ReadValidStringFor(user, "State");
            user.Zip = ReadValidIntFor(user, "Zip");

            return user;
        }

        public static Member ReadMember()
        {
            return (Member) ReadUser(new Member());
        }

        public static Provider ReadProvider()
        {
            return (Provider) ReadUser(new Provider());
        }

        public static Manager ReadManager()
        {
            return (Manager) ReadUser(new Manager());
        }

        public static Provider ReadProviderById()
        {
            Provider provider = null;
            do
            {
                Console.Write("Enter your provider ID number: ");
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

        public static Member ReadMemberById()
        {
            Member member = null;
            do
            {
                Console.Write("Enter the member ID number: ");
                int id;
                if (!int.TryParse(Console.ReadLine(), out id)) continue;
                member = Member.Collection.FindById(id);
                if (member == null)
                {
                    Console.WriteLine("Couldn't find a member with that ID number.");
                }
            } while (member == null);
            return member;
        }

        public static Manager ReadManagerById()
        {
            Manager manager = null;
            do
            {
                Console.Write("Enter the manager ID number: ");
                int id;
                if (!int.TryParse(Console.ReadLine(), out id)) continue;
                manager = Manager.Collection.FindById(id);
                if (manager == null)
                {
                    Console.WriteLine("Couldn't find a manager with that ID number");
                }
            } while (manager == null);
            return manager;
        }

        public static Service ReadServiceById()
        {
            Service service = null;
            do
            {
                Console.Write("Enter the service ID number: ");
                int id;
                if (!int.TryParse(Console.ReadLine(), out id)) continue;
                service = Service.Collection.FindById(id);
                if (service == null)
                {
                    Console.WriteLine("Couldn't find a service with that ID.");
                }
            } while (service == null);
            return service;
        }

        public static DateTime ReadDateTime(string prompt = "Enter a date")
        {
            string dateString;
            DateTime date;
            do
            {
                Console.Write(prompt + " (MM-DD-YYYY): ");
                dateString = Console.ReadLine();
            } while (!DateTime.TryParseExact(dateString, "MM-dd-yyyy",
                        EnUs, DateTimeStyles.None, out date));
            return date;
        }

        /**
         * Calls the print function on some set of models, e.g. a resultset
         * produced by Collection.FindAll()
         */
        public static void PrintAll(IEnumerable<BaseModel> items)
        {
            foreach (var item in items)
            {
                item.Print();
            }
        }
    }
}

