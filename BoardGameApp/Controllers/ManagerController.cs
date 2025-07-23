namespace BoardGameApp.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = "Manager")]
    public class ManagerController : BaseController
    {
        public IActionResult Index()
        {
            return this.Ok("I am manager!!!");
        }
    }
}
