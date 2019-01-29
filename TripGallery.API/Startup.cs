/* */
using AutoMapper;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripGallery.API.Helpers;
using System.Web.Http.Cors;
using IdentityServer3.AccessTokenValidation;
using System.IdentityModel.Tokens;

namespace TripGallery.API
{
    public class Startup
    { 

        public void Configuration(IAppBuilder app)
        {
            // Reset the mapping dictionary, ensuring the claim types aren't mapped to .net claim types
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            /* ensure the API requires a token provided by our identity server. To use that, we state that we want to use identity server bearer token authentication. As we'll pass the token from the client to the API as a bearer token. We configure it with an instance of identity server bearer token authentication options in which we state that a token must be provided by the trip gallery STS and it must contain the gallery management scope. That trip gallery STS constant refers to a location of our security token service which is the place where we host it, followed by a forward slash and identity.  The last thing we have to do is add the [authorize] attribute to our Trips controllers. This should block access to our API*/
            /*NOTE: if this is commented out, access to the API is blocked, in combination with the authorize attribute on the controller level.*/
            app.UseIdentityServerBearerTokenAuthentication(
                 new IdentityServerBearerTokenAuthenticationOptions
                 {
                     Authority = Constants.TripGallerySTS,
                     RequiredScopes = new[] { "gallerymanagement" }
                 });


            var config = WebApiConfig.Register();
            
            app.UseWebApi(config);

            InitAutoMapper();
        }

        private void InitAutoMapper()
        {
            Mapper.CreateMap<Repository.Entities.Trip,
                DTO.Trip>().ForMember(dest => dest.MainPictureUri,
                op => op.ResolveUsing(typeof(InjectImageBaseForTripResolver)));

            Mapper.CreateMap<Repository.Entities.Picture,
                DTO.Picture>()
                .ForMember(dest => dest.Uri,
                op => op.ResolveUsing(typeof(InjectImageBaseForPictureResolver)));

            Mapper.CreateMap<DTO.Picture,
              Repository.Entities.Picture>();        

            Mapper.CreateMap<DTO.Trip,
                Repository.Entities.Trip>().ForMember(dest => dest.MainPictureUri,
                op => op.ResolveUsing(typeof(RemoveImageBaseForTripResolver))); ;

            Mapper.CreateMap<DTO.PictureForCreation,
                Repository.Entities.Picture>()
                .ForMember(o => o.Id, o => o.Ignore())
                .ForMember(o => o.TripId, o => o.Ignore())
                .ForMember(o => o.OwnerId, o => o.Ignore())
                .ForMember(o => o.Uri, o => o.Ignore());


            Mapper.CreateMap<DTO.TripForCreation,
         Repository.Entities.Trip>()
            .ForMember(o => o.Id, o => o.Ignore())
            .ForMember(o => o.MainPictureUri, o => o.Ignore())
            .ForMember(o => o.Pictures, o => o.Ignore())
            .ForMember(o => o.OwnerId, o => o.Ignore());

            Mapper.AssertConfigurationIsValid();
        }
    }
}
