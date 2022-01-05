using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string RetrieveEmailFromPrincipal(this ClaimsPrincipal user)
        {
            //return user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            // We can also use below as it is simplified
            return user.FindFirstValue(ClaimTypes.Email);
        }
    }
}
