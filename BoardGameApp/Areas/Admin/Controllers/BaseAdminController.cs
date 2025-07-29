namespace BoardGameApp.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using static BoardGameApp.GCommon.ApplicationConstants;

    [Area(RoleAdmin)]
    [Authorize(Roles = RoleAdmin)]
    public abstract class BaseAdminController : Controller
    {
        private bool IsUserAuthenticated()
        {
            bool retRes = false;
            if (this.User.Identity != null)
            {
                retRes = this.User.Identity.IsAuthenticated;
            }

            return retRes;
        }

        protected Guid? GetUserId()
        {
            string? userIdString = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userIdString, out Guid userId))
            {
                return userId;
            }

            return null;
        }
    }
}
