using System;
using LiteDB;

namespace ChocAn
{
    class MainClass
    {
        private static int ReadChoice() {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("-- ChocAn Menu ------------------------------");
            Console.ResetColor();
            Console.WriteLine("1. Display all members");
            Console.WriteLine("2. Display all providers");
            Console.WriteLine("3. Add a member");
            Console.WriteLine("4. Add a provider");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("-- Testing Functions ------------------------");
            Console.ResetColor();
            Console.WriteLine("5. Empty the database");
            Console.WriteLine("6. Seed the database with some fake test data");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("---------------------------------------------");
            Console.ResetColor();
            Console.WriteLine("7. Quit");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Your choice: ");
            Console.ResetColor();
            var input = Console.ReadLine();
            return Convert.ToInt32(input);
        }

        public static void Main (string[] args)
        {
            Console.Clear();
            int choice = 0;
            while (choice != 7) {
                choice = ReadChoice();
                switch (choice) {
                    case 1:
                        Controller.PrintAllMembers();
                        break;
                    case 2:
                        Controller.PrintAllProviders();
                        break;
                    case 3:
                        Controller.CreateMember();
                        break;
                    case 4:
                        Controller.CreateProvider();
                        break;
                    case 5:
                        Controller.ClearAllData();
                        break;
                    case 6:
                        Controller.SeedData();
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
