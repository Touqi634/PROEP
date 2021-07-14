using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace webApp
{ 
    public static class Utils
    {
        [Authorize]
        public static string GetCurrentUserId(ControllerBase controllerBase)
        {
            if (controllerBase.User != null)
            {
                return controllerBase.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            return "unknown";
        }

       
    }
}