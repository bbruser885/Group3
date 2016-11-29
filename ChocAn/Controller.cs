using System;
using System.Collections.Generic;

namespace ChocAn
{
    public sealed class Controller
    {
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
                    SingleInstance.SeedUserData();
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
            var provider = View.ReadProviderById();
            var member = View.ReadMemberById();
            var service = View.ReadServiceById();
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
	    //Consultation was created, Write copy to file
	    writeConsultationToFile(consultation, addMember);
		
		
        }

        public void RequestDirectory()
        {
            View.PrintError("Not implemented yet.");
        }

        public void RunReport()
        {
            View.PrintError("Not implemented yet.");
        }

        public void RunAllReports()
        {
            View.PrintError("Not implemented yet.");
        }

        public void CreateUser()
        {
            View.PrintError("Not implemented yet.");
        }

        public void EditUser()
        {
            View.PrintError("Not implemented yet.");
        }

        public void DeleteUser()
        {
            View.PrintError("Not implemented yet.");
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
	    System.IO.Directory.CreateDirectory("Group3/ChocAn/consultationFiles/");
		System.IO.File.WriteAllLines(@"Group3/ChocAn/consultationFiles/" + fileName , consultation);
	}
	
        /**
         * Test method: Seed the database with some fake user data
         */
        public void SeedUserData()
		{

            Member.Collection.Insert(new Member {
                    Name = "Howard Price",
                    Street = "1373 Armistice Circle",
                    City = "Memphis",
                    State = "TN",
                    Zip = 38161,
                    });
            Member.Collection.Insert(new Member {
                    Name = "Robin Smith",
                    Street = "94565 Sundown Ct",
                    City = "Las Vegas",
                    State = "NV",
                    Zip = 89105,
                    });
            Member.Collection.Insert(new Member {
                    Name = "Donna Hernandez",
                    Street = "9 Nancy St",
                    City = "Dayton",
                    State = "OH",
                    Zip = 45414,
                    });
            Provider.Collection.Insert(new Provider {
                    Name = "Phillip Parker",
                    Street = "982 Express Point",
                    City = "Austin",
                    State = "TX",
                    Zip = 78710,
                    });
            Provider.Collection.Insert(new Provider {
                    Name = "Diana Brown",
                    Street = "2157 Moose Crossing",
                    City = "Muskegon",
                    State = "MI",
                    Zip = 49444,
                    });
            Manager.Collection.Insert(new Manager {
                    Name = "Bob Dole",
                    Street = "123 Fake sreet",
                    City = "The Shire",
                    State= "Hobbiton",
                    Zip = 12313,
                    });
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

