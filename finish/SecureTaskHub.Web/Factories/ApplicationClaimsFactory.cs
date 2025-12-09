using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace SecureTaskHub.Web.Factories
{
    public class ApplicationClaimsFactory : UserClaimsPrincipalFactory<IdentityUser>
    {
        public ApplicationClaimsFactory(UserManager<IdentityUser> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }
         
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            // Add custom claims
            identity.AddClaim(new Claim("Department", GetDepartmentForUser(user.Email ?? "")));
            return identity;
        }

        private static string GetDepartmentForUser(string email)
        {
            return email switch
            {
                "admin@demo.local" => "Administration",
                "alice@demo.local" => "Engineering",
                "bob@demo.local" => "IT",
                _ => ""
            };
        }
    }
}
