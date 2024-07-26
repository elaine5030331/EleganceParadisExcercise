using Microsoft.Identity.Client;
using System.Security.Claims;

namespace EleganceParadisAPI.Helpers
{
    public static class ClaimsPrincipalExtension
    {
        public static int? GetAccountId(this ClaimsPrincipal user)
        {
            var claim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.PrimarySid);
            if (claim == null) return null;
            if (!int.TryParse(claim.Value, out int id)) return null;
            return id;
        }
    }
}
