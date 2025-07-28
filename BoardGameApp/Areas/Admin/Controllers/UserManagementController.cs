namespace BoardGameApp.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class UserManagementController : BaseAdminController
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
