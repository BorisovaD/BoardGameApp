namespace BoardGameApp.Controllers
{
    using BoardGameApp.Services.Core;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.GameSession;
    using BoardGameApp.Web.ViewModels.Reservation;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;


    [Authorize]
    public class ReservationController : BaseController
    {
        private readonly IReservationService reservationService;
        public ReservationController(IReservationService reservationService)
        {
            this.reservationService = reservationService;
        }

        [HttpGet]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Index()
        {            
            try
            {
                IEnumerable<ReservationIndexViewModel> activeReservations = await reservationService.GetAllActiveReservations();
                return View(activeReservations);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index), "Home");
            }
        }
                
        [HttpGet]
        public async Task<IActionResult> MyReservations()
        {            
            try
            {
                Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                IEnumerable<ReservationIndexViewModel> model = await reservationService.GetMyReservationsAsync(userId);
                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index), "Home");
            }
        }
    }
}
