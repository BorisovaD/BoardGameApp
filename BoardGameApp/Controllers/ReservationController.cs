namespace BoardGameApp.Controllers
{
    using BoardGameApp.Services.Core;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.GameSession;
    using BoardGameApp.Web.ViewModels.Reservation;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    public class ReservationController : BaseController
    {
        private readonly IReservationService reservationService;
        public ReservationController(IReservationService reservationService)
        {
            this.reservationService = reservationService;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<ReservationIndexViewModel> activeReservations = await reservationService.GetAllActiveReservations();
            return View(activeReservations);
        }

        [Authorize]
        public async Task<IActionResult> MyReservations()
        {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            IEnumerable<ReservationIndexViewModel> model = await reservationService.GetMyReservationsAsync(userId);
            return View(model);
        }
    }
}
