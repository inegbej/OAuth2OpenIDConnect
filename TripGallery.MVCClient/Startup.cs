﻿using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using TripGallery.MVCClient.Helpers;


[assembly: OwinStartup(typeof(TripGallery.MVCClient.Startup))]
namespace TripGallery.MVCClient
{
    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            // Reset the mapping dictionary, ensuring the claim types aren't mapped to .net claim types
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            // Enable antiforgery token
            AntiForgeryConfig.UniqueClaimTypeIdentifier = IdentityModel.JwtClaimTypes.Name;

            // Add cooking pipeline to store the accessToken
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                ExpireTimeSpan = new TimeSpan(0, 30, 0),
                SlidingExpiration = true
            });

            // Add the openID Connect middleware for authentication
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {

                ClientId = "tripgalleryhybrid",
                Authority = Constants.TripGallerySTS,
                RedirectUri = Constants.TripGalleryMVC,
                SignInAsAuthenticationType = "Cookies",
                ResponseType = "code id_token token",
                Scope = "openid profile address gallerymanagement roles offline_access",
                UseTokenLifetime = false,
                PostLogoutRedirectUri = Constants.TripGalleryMVC,  // for SSout

                //This allows us to inspect the token we received i.e when the token has been validated
                Notifications = new OpenIdConnectAuthenticationNotifications()
                {
                    SecurityTokenValidated = async n =>
                    {
                        Helpers.TokenHelper.DecodeAndWrite(n.ProtocolMessage.IdToken);
                        Helpers.TokenHelper.DecodeAndWrite(n.ProtocolMessage.AccessToken);  // save the access token

                        var givenNameClaim = n.AuthenticationTicket
                            .Identity.FindFirst(IdentityModel.JwtClaimTypes.GivenName);

                        var familyNameClaim = n.AuthenticationTicket
                            .Identity.FindFirst(IdentityModel.JwtClaimTypes.FamilyName);

                        var subClaim = n.AuthenticationTicket
                            .Identity.FindFirst(IdentityModel.JwtClaimTypes.Subject);

                        // create a new claims, issuer + sub as unique identifier
                        var nameClaim = new Claim(IdentityModel.JwtClaimTypes.Name,
                                    Constants.TripGalleryIssuerUri + subClaim.Value);

                        var newClaimsIdentity = new ClaimsIdentity(
                           n.AuthenticationTicket.Identity.AuthenticationType,
                           IdentityModel.JwtClaimTypes.Name,
                           IdentityModel.JwtClaimTypes.Role);

                        if (nameClaim != null)
                        {
                            newClaimsIdentity.AddClaim(nameClaim);
                        }

                        if (givenNameClaim != null)
                        {
                            newClaimsIdentity.AddClaim(givenNameClaim);
                        }

                        if (familyNameClaim != null)
                        {
                            newClaimsIdentity.AddClaim(familyNameClaim);
                        }

                        // request a refresh token
                        var tokenClientForRefreshToken = new TokenClient(
                                   Constants.TripGallerySTSTokenEndpoint,
                                   "tripgalleryhybrid",
                                   Constants.TripGalleryClientSecret);

                        // create a new token client to the token endpoint passing it our clientId and secret
                        var refreshResponse = await
                            tokenClientForRefreshToken.RequestAuthorizationCodeAsync(
                            n.ProtocolMessage.Code,
                            Constants.TripGalleryMVC);

                        // Fire a request passing in the authorization code and the redirect uri. This return a response for our refresh token. allow us to save the token for use later on.
                        var expirationDateAsRoundtripString
                            = DateTime.SpecifyKind(DateTime.UtcNow.AddSeconds(refreshResponse.ExpiresIn)
                            , DateTimeKind.Utc).ToString("o");   // o format fdor the date technique, preserve the time for all local and universal times

                        // Saves the time the access token expires. allows us for easier checks later on
                        newClaimsIdentity.AddClaim(new Claim("id_token", refreshResponse.IdentityToken));
                        newClaimsIdentity.AddClaim(new Claim("refresh_token", refreshResponse.RefreshToken));
                        newClaimsIdentity.AddClaim(new Claim("access_token", refreshResponse.AccessToken));
                        newClaimsIdentity.AddClaim(new Claim("expires_at", expirationDateAsRoundtripString));

                        // add the access token so we can access it later on 
                        //newClaimsIdentity.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));

                        // create a new authentication ticket, overwriting the old one.
                        n.AuthenticationTicket = new AuthenticationTicket(
                                                 newClaimsIdentity,
                                                 n.AuthenticationTicket.Properties);
                    },
                    RedirectToIdentityProvider = async n =>
                    {
                        // get id token to add as id token hint
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            var identityTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                            if (identityTokenHint != null)
                            {
                                n.ProtocolMessage.IdTokenHint = identityTokenHint.Value;
                            }

                        }
                        else if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.AuthenticationRequest)
                        {
                            string existingAcrValues = null;
                            if (n.ProtocolMessage.Parameters.TryGetValue("acr_values", out existingAcrValues))
                            {
                                // add "2fa" - acr_values are separated by a space
                                n.ProtocolMessage.Parameters["acr_values"] = existingAcrValues + " 2fa";
                            }

                            n.ProtocolMessage.Parameters["acr_values"] = "2fa";
                        }
                    }
                }
            });

        }
    }
}
