using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TripCompany.IdentityServer.Models
{
    /*This class will allow a user to enter Fname, Lname etc, chose a role i.e FreeUser, PayingUser - in a form*/
    public class CreateUserAccountModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public List<SelectListItem> Roles { get; set; }    // Allow the user to chose a subscription that is matched to a role i.e freeuser or paying user

        // ctor
        public CreateUserAccountModel()
        {
            Roles = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Free version", Value = "FreeUser"},
                new SelectListItem() { Text = "Plus version", Value = "PayingUser"}
            };
        }
    }
}