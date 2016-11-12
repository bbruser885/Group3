using System;
using LiteDB;

namespace ChocAn
{
    class MainClass
    {
        private static int ReadChoice()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("-- ChocAn Menu ------------------------------");
            Console.ResetColor();
            Console.WriteLine("1. Display all members");
            Console.WriteLine("2. Display all providers");
            Console.WriteLine("3. Display all services");
            Console.WriteLine("4. Display all consultations");
            Console.WriteLine("5. Add a member");
            Console.WriteLine("6. Add a provider");
            Console.WriteLine("7. Create a consultation");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("-- Testing Functions ------------------------");
            Console.ResetColor();
            Console.WriteLine("8. Empty the user data");
            Console.WriteLine("9. Seed the database with some fake user data");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("---------------------------------------------");
            Console.ResetColor();
            Console.WriteLine("0. Quit");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Your choice: ");
            Console.ResetColor();
            int c;
            if (!int.TryParse (Console.ReadLine (), out c)) return -1;
            return c;
        }

        public static void Main (string[] args)
        {
            // Initialize the database with the service catalog data
            BaseModel.InitializeDatabase();
            Console.Clear();
            int choice = -1;
            while (choice != 0) {
                choice = ReadChoice();
                switch (choice) {
                    case 1:
                        Controller.PrintAllMembers();
                        break;
                    case 2:
                        Controller.PrintAllProviders();
                        break;
                    case 3:
                        Controller.PrintAllServices();
                        break;
                    case 4:
                        Controller.PrintAllConsultations();
                        break;
                    case 5:
                        Controller.CreateMember();
                        break;
                    case 6:
                        Controller.CreateProvider();
                        break;
                    case 7:
                        Controller.CreateConsultation();
                        break;
                    case 8:
                        BaseModel.ClearUserData();
                        break;
                    case 9:
                        Controller.SeedUserData();
                        break;
                    default:
                        break;
                }
                Console.WriteLine();
            }
            Environment.Exit(0);
        }
    }
}
