using System;
using System.Globalization;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ChocAn
{
    public class View
    {
        // User type enum for use in factory methods
        public enum UserTypes { Member, Provider }

        /**
         * Use the validation annotations on a model to get valid inputs from
         * the Console. This could stand a refactor. Doesn't currently work for
         * boolean fields, but numbers and strings are OK.
         * (See also MS's docs for System.ComponentModel.DataAnnotations)
         *
         * Params:
         *    modelInstance: an instance of a class (Member, Provider, etc.)
         *    propertyName: a string with the name of the property you want to set
         *
         * Returns: a valid value for the property you passed in
         *
         * Example:
         *    Member m = new Member();
         *    m.Name = ReadValidPropertyFor(m, "Name");
         *    // m now has a Name meeting validation requirements
         */
        protected static object ReadValidPropertyFor(object modelInstance, string propertyName) {
            var context = new ValidationContext(modelInstance) { MemberName = propertyName };
            object propertyValue;
            bool valid = false;
            Type propertyType = (Type)modelInstance.GetType().GetProperty(propertyName).PropertyType;
            do {
                // Use the property's name to prompt the user
                Console.Write(string.Format("{0}: ", propertyName));
                propertyValue = Console.ReadLine();

                // Cast the input string to the right type for this property
                propertyValue = Convert.ChangeType(propertyValue, propertyType);

                // Run the validation and print results
                var results = new List<ValidationResult>();
                valid = Validator.TryValidateProperty(propertyValue, context, results);
                foreach (var result in results) {
                    Console.WriteLine(result);
                }
            } while (!valid);
            return propertyValue;
        }

        /**
         * Factory method of sorts to initialize a user of the given type.
         * Allows creating instances of objects that inherit from BaseUser
         * without having to duplicate all the initialization code.
         *
         * Params:
         * type: One of the user models from the UserTypes enum above.
         *       Determines which type of model will be returned.
         *
         * Returns: an initialized instance of the type you passed in
         */
        protected static BaseUser ReadUser(UserTypes type) {
            BaseUser user;
            if (type == UserTypes.Member) {
                user = new Member();
            } else if (type == UserTypes.Provider) {
                user = new Provider();
            } else {
                throw new Exception("Invalid user type passed to ReadUser");
            }

            Console.WriteLine();

            user.Name = (string)ReadValidPropertyFor(user, "Name");
            user.Street = (string)ReadValidPropertyFor(user, "Street");
            user.City = (string)ReadValidPropertyFor(user, "City");
            user.State = (string)ReadValidPropertyFor(user, "State");
            user.Zip = (int)ReadValidPropertyFor(user, "Zip");

            return user;
        }

        public static Member ReadMember() {
            return (Member)ReadUser(UserTypes.Member);
        }

        public static Provider ReadProvider() {
            return (Provider)ReadUser(UserTypes.Provider);
        }

        public static Provider ReadProviderById() {
            Provider provider;
            do {
                Console.Write("Enter your provider ID number: ");
                int id = Convert.ToInt32(Console.ReadLine());
                provider = Provider.Collection.FindById(id);
                if (provider == null) {
                    Console.WriteLine("Couldn't find a provider with that ID.");
                }
            } while (provider == null);
            return provider;
        }

        public static Member ReadMemberById() {
            Member member;
            do {
                Console.Write("Enter the member ID number: ");
                int id = Convert.ToInt32(Console.ReadLine());
                member = Member.Collection.FindById(id);
                if (member == null) {
                    Console.WriteLine("Couldn't find a member with that ID.");
                }
            } while (member == null);
            return member;
        }

        public static Service ReadServiceById() {
            Service service;
            do {
                Console.Write("Enter the service ID number: ");
                int id = Convert.ToInt32(Console.ReadLine());
                service = Service.Collection.FindById(id);
                if (service == null) {
                    Console.WriteLine("Couldn't find a service with that ID.");
                }
            } while (service == null);
            return service;
        }

        public static DateTime ReadDateTime() {
            string dateString;
            DateTime date;
            CultureInfo enUS = new CultureInfo("en-US");
            do {
                Console.Write("Enter a date (MM-DD-YYYY): ");
                dateString = Console.ReadLine();
            } while (!DateTime.TryParseExact(
                dateString, "MM-dd-yyyy", enUS, DateTimeStyles.None, out date)
            );
            return date;
        }

        /**
         * Calls the print function on some set of models, e.g. a resultset 
         * produced by Collection.FindAll()
         */
        public static void PrintAll(IEnumerable<BaseModel> items) {
            foreach (var item in items) {
                item.Print();
            }
        }
    }
}

