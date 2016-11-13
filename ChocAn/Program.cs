using System;
using LiteDB;

namespace ChocAn
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            // Initialize the database with the service catalog data
            BaseModel.InitializeDatabase();
            Console.Clear();
            View.MainMenuLoop();
            Environment.Exit(0);
        }
    }
}
