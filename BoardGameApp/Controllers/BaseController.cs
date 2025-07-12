namespace BoardGameApp.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    public abstract class BaseController : Controller
    {
        [Authorize]
        protected bool IsUserAuthenticated()
        {
            return this.User.Identity?.IsAuthenticated ?? false;
        }

        protected Guid? GetUserId()
        {
            if (!IsUserAuthenticated())
            {
                return null;
            }

            string? idString = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(idString, out var parsedGuid))
            {
                return parsedGuid;
            }

            return null;
        }
    }
}
