using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChocAn
{
    public enum LogInOptions
    {
        Exit = 0,
        Manager,
        Provider,
        SeedData,
    }
    public static class LogInHelpers
    {
        public static string GetString(this LogInOptions option)
        {
            if (option == LogInOptions.Exit)
                return "Exit";
            else if (option == LogInOptions.Manager)
                return "Manager";
            else if (option == LogInOptions.Provider)
                return "Provider";
            else if (option == LogInOptions.SeedData)
                return "Seed Data";
            else
                return "None";
        }
    }
}
