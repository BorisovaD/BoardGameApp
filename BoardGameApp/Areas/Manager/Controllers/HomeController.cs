namespace BoardGameApp.Areas.Manager.Controllers
{
    using BoardGameApp.Controllers;
    using BoardGameApp.GCommon;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Area("Manager")]
    [Authorize(Roles = ApplicationConstants.RoleManager)]
    public class HomeController : BaseController
    {
        [HttpGet]
        public IActionResult Manage()
        {
            return View();
        }
    }
}
