using System.Security.Claims;

namespace CommunicationAPI.Extension
{
    public static class ClaimsPrincipalExtensions
    {
        public static string getUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name).Value;
        }

        public static string getUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
