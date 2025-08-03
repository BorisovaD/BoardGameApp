namespace BoardGameApp.Controllers
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Services.Core;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.BoardGame;
    using BoardGameApp.Web.ViewModels.Reservation;
    using BoardGameApp.Web.ViewModels.Ticket;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class TicketController : BaseController
    {
        private readonly ITicketService ticketService;
        private readonly UserManager<BoardgameUser> userManager;

        public TicketController(ITicketService ticketService, UserManager<BoardgameUser> userManager)
        {
            this.ticketService = ticketService;
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<TicketIndexViewModel> activeTickets = await
                ticketService.GetAllTickets();

                return View(activeTickets);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            try
            {
                Guid? userId = this.GetUserId();
                TicketDetailsViewModel ticketDetails = await 
                    this.ticketService.GetTicketDetailsAsync(id, userId);
                if (ticketDetails == null)
                {
                    return this.RedirectToAction(nameof(Index));
                }

                return this.View(ticketDetails);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Buy(Guid gameSessionId)
        {
            try
            {
                var viewModel = await ticketService.GetTicketInfoAsync(gameSessionId);

                return View(viewModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Buy(BuyTicketViewModel model)
        {
            try
            {
                string userIdString = userManager.GetUserId(User);

                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return Unauthorized();
                }

                bool success = await ticketService.BuyTicketAsync(userId, model.GameSessionId, model.TicketsToBuy);

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Not enough available tickets.");

                    model = await ticketService.GetTicketInfoAsync(model.GameSessionId);

                    return View(model);
                }

                return RedirectToAction("MyReservations", "Reservation");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index), "Home");
            }            
        }
    }
}
