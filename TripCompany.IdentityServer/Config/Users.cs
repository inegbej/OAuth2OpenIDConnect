﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core;
using IdentityServer3.Core.Services.InMemory;

namespace TripCompany.IdentityServer.Config
{
    public static class Users
    {
        public static List<InMemoryUser> Get()
        {
            // we are going to start working with 2 users. The "subject value" is the user unique identifier
            return new List<InMemoryUser>() {
                 
                new InMemoryUser
	            {
	                Username = "Kevin",
	                Password = "secret",                    
	                Subject = "b05d3546-6ca8-4d32-b95c-77e94d705ddf",
                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "Kevin"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Dockx"),
                        new Claim(Constants.ClaimTypes.Address, "1, Main Street, Antwerp, Belgium"),
                        new Claim("role", "PayingUser")
                    }
	             }
	            ,
	            new InMemoryUser
	            {
	                Username = "Sven",
	                Password = "secret",
	                Subject = "bb61e881-3a49-42a7-8b62-c13dbe102018",
                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "Sven"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Vercauteren"),
                        new Claim(Constants.ClaimTypes.Address, "2, Main Road, Antwerp, Belgium"),
                        new Claim("role", "FreeUser")
                    }
                }  
            };
        }
    }

}
