using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Configuration;
using TripCompany.IdentityServer.Config;
using IdentityServer3.Core.Services.Default;
using TripCompany.IdentityServer.Services;
using IdentityServer3.Core.Services;
using Microsoft.Owin.Security.Facebook;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Owin.Security.WsFederation;

namespace TripCompany.IdentityServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // enable CORS
            var corsPolicyService = new DefaultCorsPolicyService()
            {
                AllowAll = true
            };

            /*Now let's ensure we startup identity server with the correct configuration so it uses what we just added. */
            /*Here we are mapping to a certain URI\identity with app.map we can map that \identity URI to the identity server app and configure it. To startup identity server, we can use a factor and pass that into the options used for configuring identity server. When configuring this factory, we can state where the clients, scopes, and users come from. */
            app.Map("/identity", idsrvApp =>
            {
                // here we are configuring a security token service (STS)
                var idServerServiceFactory = new IdentityServerServiceFactory()
                                .UseInMemoryClients(Clients.Get())
                                .UseInMemoryScopes(Scopes.Get());
                //.UseInMemoryUsers(Users.Get());    // we can now start using our CustomUserService().

                // do not cache the views
                var defaultViewServiceOptions = new DefaultViewServiceOptions();
                defaultViewServiceOptions.CacheViews = false;

                // Register CORS
                idServerServiceFactory.CorsPolicyService = new
                    Registration<IdentityServer3.Core.Services.ICorsPolicyService>(corsPolicyService);

                // use our custom UserService
                var customUserService = new CustomUserService();
                idServerServiceFactory.UserService = new Registration<IUserService>(resolver => customUserService);

                // create an identityserver option instance
                var options = new IdentityServerOptions
                {
                    Factory = idServerServiceFactory,
                    SiteName = "TripCompany Security Token Service",                    
                    IssuerUri = TripGallery.Constants.TripGalleryIssuerUri,
                    PublicOrigin = TripGallery.Constants.TripGallerySTSOrigin,
                    SigningCertificate = LoadCertificate(),
                    AuthenticationOptions = new AuthenticationOptions()
                    {
                        EnablePostSignOutAutoRedirect = true,               //enable single-sign-out
                        //PostSignOutAutoRedirectDelay = 2                    // 2 seconds delay
                        LoginPageLinks = new List<LoginPageLink>()          // link for registration
                        {
                            new LoginPageLink()
                            {
                                Type= "createaccount",
                                Text = "Create a new account",
                                Href = "~/createuseraccount"
                            }
                        },
                        IdentityProviders = ConfigureAdditionalIdProviders
                    },
                    CspOptions = new CspOptions()
                    {
                        Enabled = false
                        // once available, leave Enabled at true and use:
                        // FrameSrc = "https://localhost:44318 https://localhost:44316"
                        // or
                        // FrameSrc = "*" for all URI's.
                    }
                };

                idsrvApp.UseIdentityServer(options);
            });                      
        }


        /* Configure External Identity*/
        private void ConfigureAdditionalIdProviders(IAppBuilder appBuilder, string signInAsType)
        {
            // Using Facebook Authentication
            var fbAuthOptions = new FacebookAuthenticationOptions
            {
                AuthenticationType = "Facebook",
                SignInAsAuthenticationType = signInAsType,
                AppId = "895739530475035",
                AppSecret = "af8fb8900e65ebd7b7056265d130c3ee",
                Provider = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationProvider()
                {
                    OnAuthenticated = (context) =>
                    {
                        using (var client = new HttpClient())
                        {
                            // get claims from FB's graph 

                            var result = client.GetAsync("https://graph.facebook.com/me?fields=first_name,last_name,email&access_token="
                              + context.AccessToken).Result;

                            if (result.IsSuccessStatusCode)
                            {
                                var userInformation = result.Content.ReadAsStringAsync().Result;
                                var fbUser = JsonConvert.DeserializeObject<FacebookUser>(userInformation);

                                context.Identity.AddClaim(new System.Security.Claims.Claim(
                                    IdentityServer3.Core.Constants.ClaimTypes.GivenName, fbUser.first_name));
                                context.Identity.AddClaim(new System.Security.Claims.Claim(
                                    IdentityServer3.Core.Constants.ClaimTypes.FamilyName, fbUser.last_name));
                                context.Identity.AddClaim(new System.Security.Claims.Claim(
                                 IdentityServer3.Core.Constants.ClaimTypes.Email, fbUser.email));

                                //// there's no role concept...
                                //context.Identity.AddClaim(new System.Security.Claims.Claim(
                                //    "role", "FreeUser"));
                            }

                        }

                        return Task.FromResult(0);
                    }
                }
            };
            fbAuthOptions.Scope.Add("email");

            appBuilder.UseFacebookAuthentication(fbAuthOptions);


            // Using Windows Authentication
            var windowsAuthentication = new WsFederationAuthenticationOptions
            {
                AuthenticationType = "windows",
                Caption = "Windows",
                SignInAsAuthenticationType = signInAsType,
                MetadataAddress = "https://localhost:44330/",
                Wtrealm = "urn:win"

            };
            appBuilder.UseWsFederationAuthentication(windowsAuthentication);
        }


        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                string.Format(@"{0}\certificates\idsrv3test.pfx",
                AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }
    }
}
