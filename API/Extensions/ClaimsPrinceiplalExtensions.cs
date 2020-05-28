using System.Linq;
using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrinceiplalExtensions
    {
        public static string RetreiveEmailFromPrincipal(this ClaimsPrincipal user)
        {
            return user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?
            .Value;
        }
    }
}