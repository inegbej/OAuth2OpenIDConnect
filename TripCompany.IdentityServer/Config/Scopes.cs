/* So there's two types of scopes, our identity scopes and resource scopes. Resource scope is for authorization using oauth. A token will contain this scopes to have access to the API. 
   One way of looking at scope is the intent of the client, ie I the client ask you, the user or resource owner, to grant me access to your name. Or to the API you own. */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;

namespace TripCompany.IdentityServer.Config
{
    public static class Scopes
    {
        public static IEnumerable<Scope> Get()
        {

            return new List<Scope>
                {
                    StandardScopes.OpenId,                // openid for client application - authentication
                    StandardScopes.ProfileAlwaysInclude,
                    StandardScopes.Address,               // added an address scope
                    StandardScopes.OfflineAccess,         // added a refresh token scope via the offline property
                    new Scope                             // OAuth for API - authorization: Resource scope
                    {
                        Name = "gallerymanagement",
                        //DisplayName = "Gallery Management",
                        Description = "Allow the application to manage galleries on your behalf.",
                        Type = ScopeType.Resource,
                        Claims = new List<ScopeClaim>()
                        {
                            new ScopeClaim("role", false)
                        }
                    },
                    new Scope
                    {
                        Name = "roles",
                        DisplayName = "Role(s)",
                        Description = "Allow the application to see your role(s).",
                        Type = ScopeType.Identity,
                        Claims = new List<ScopeClaim>()
                        {
                            new ScopeClaim("role", true)
                        }
                    }


                };
        }
    }
}
