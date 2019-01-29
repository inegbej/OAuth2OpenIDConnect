using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripCompany.Repository.Entities
{
    public class User
    {
        public string Subject { get; set; }

        public string UserName { get; set; }

        // Note from @KevinDockx: 
        // Demo purposes only!!  For real-life situations, always encrypt 
        // your passwords with an algorithm that does not allow de-encryption!
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public IList<UserClaim> UserClaims { get; set; }

        public IList<UserLogin> UserLogins { get; set; }   //link user account to external identity providers 

        public User()
        {
            UserClaims = new List<UserClaim>();
            UserLogins = new List<UserLogin>();
        }
    }
}
