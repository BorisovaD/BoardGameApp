namespace BoardGameApp.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseAdminController
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
