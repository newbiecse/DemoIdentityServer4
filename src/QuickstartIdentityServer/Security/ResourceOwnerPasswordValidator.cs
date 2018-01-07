using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using QuickstartIdentityServer.Model;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuickstartIdentityServer.Security
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private UserManager<ApplicationUser> _userManager;

        public ResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _userManager.FindByNameAsync(context.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, context.Password))
            {
                var claims = GetUserClaims(user);

                context.Result = new GrantValidationResult(
                    subject: user.Id.ToString(),
                    authenticationMethod: "password",
                    claims: claims);
                //context.Result = new GrantValidationResult(await _userManager.GetUserIdAsync(user), "password");
                return;
            }

            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Incorrect username or password");
        }

        public static Claim[] GetUserClaims(ApplicationUser user)
        {
            return new Claim[]
            {
                new Claim("user_id", user.Id.ToString() ?? ""),
                //new Claim(JwtClaimTypes.Name, (!string.IsNullOrEmpty(user.Firstname) && !string.IsNullOrEmpty(user.Lastname)) ? (user.Firstname + " " + user.Lastname) : ""),
                //new Claim(JwtClaimTypes.GivenName, user.Firstname  ?? ""),
                //new Claim(JwtClaimTypes.FamilyName, user.Lastname  ?? ""),
                new Claim(JwtClaimTypes.Email, user.Email  ?? ""),
                //new Claim("some_claim_you_want_to_see", user.Some_Data_From_User ?? ""),

                //roles
                //new Claim(JwtClaimTypes.Role, user.Role)
            };
        }
    }
}
