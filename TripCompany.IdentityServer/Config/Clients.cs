using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using TripGallery;

namespace TripCompany.IdentityServer.Config
{
    public static class Clients
    {
        public static IEnumerable<Client> Get()
        {
            // defining OAuth Clients
            return new[]
             {
                new Client
                {
                    // clientId - used for our mvc application
                     ClientId = "tripgalleryclientcredentials",
                     ClientName = "Trip Gallery (Client Credentials)",
                     Flow = Flows.ClientCredentials,
                     AllowAccessToAllScopes = true,
                     // client secret
                    ClientSecrets = new List<Secret>()
                    {
                        new Secret(TripGallery.Constants.TripGalleryClientSecret.Sha256())
                    }
                }
                ,
                // used for our Angular application Demo
                new Client
                {
                     ClientId = "tripgalleryimplicit",
                     ClientName = "Trip Gallery (Implicit)",
                     Flow = Flows.Implicit,
                     AllowAccessToAllScopes = true,
                     IdentityTokenLifetime = 10,     // 10 seconds for identity token lifetime
                     AccessTokenLifetime = 120,      // 120 seconds for access token lifetime
                     RequireConsent = false,         // to achive sigle sign-on  (SSOn)

                    // redirect = URI of the Angular application
                    RedirectUris = new List<string>
                    {
                        TripGallery.Constants.TripGalleryAngular + "callback.html",
                        // for silent refresh
                        TripGallery.Constants.TripGalleryAngular + "silentrefreshframe.html"
                    },
                    // to achive sigle sign-out  (SSOut)
                    PostLogoutRedirectUris = new List<string>()
                    {
                         TripGallery.Constants.TripGalleryAngular + "index.html"
                    }
                },

                new Client
                {
                     ClientId = "tripgalleryauthcode",
                     ClientName = "Trip Gallery (Authorization Code)",
                     Flow = Flows.AuthorizationCode,
                     AllowAccessToAllScopes = true,

                    // redirect = URI of the MVC application callback
                    RedirectUris = new List<string>
                    {
                        TripGallery.Constants.TripGalleryMVCSTSCallback
                    },           

                    // client secret
                    ClientSecrets = new List<Secret>()
                    {
                        new Secret(TripGallery.Constants.TripGalleryClientSecret.Sha256())
                    }
                },

                new Client
                {
                     ClientId = "tripgalleryropc",
                     ClientName = "Trip Gallery (Resource Owner Password Credentials)",
                     Flow = Flows.ResourceOwner,
                     AllowAccessToAllScopes = true,

                    ClientSecrets = new List<Secret>()
                    {
                        new Secret(TripGallery.Constants.TripGalleryClientSecret.Sha256())
                    }
                },

                new Client
                {
                     ClientId = "tripgalleryhybrid",
                     ClientName = "Trip Gallery (Hybrid)",
                     Flow = Flows.Hybrid,
                     AllowAccessToAllScopes = true,
                     IdentityTokenLifetime = 10,     // 10 seconds for identity token lifetime
                     AccessTokenLifetime = 120,      // 120 seconds for access token lifetime 
                     RequireConsent = false,         // to achive sigle sign-on  (SSO)

                     // redirect = URI of the MVC application
                    RedirectUris = new List<string>()
                    {
                        TripGallery.Constants.TripGalleryMVC
                    },

                    // for the refreshtoken we have to add the secret
                    ClientSecrets = new List<Secret>()
                    {
                        new Secret(TripGallery.Constants.TripGalleryClientSecret.Sha256())
                    },
                    // to achive sigle sign-out  (SSOut)
                    PostLogoutRedirectUris = new List<string>()
                    {
                         TripGallery.Constants.TripGalleryMVC
                    }

                }

             };
        }
    }
}
