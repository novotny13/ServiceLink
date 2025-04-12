using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoServis360
{
    
        public class Mechanic
        {
            public int MechanicID { get; set; }
        public int UserID { get; set; }
        public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Specialization { get; set; }
          

            public Mechanic(int mechanicID, int userID, string firstName, string lastName, string specialization)
            {
                MechanicID = mechanicID;
                UserID = userID;
                FirstName = firstName;
                LastName = lastName;
                Specialization = specialization;
               
            }
        }
    }

