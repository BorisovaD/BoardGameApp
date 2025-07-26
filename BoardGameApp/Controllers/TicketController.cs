namespace BoardGameApp.Controllers
{
    using BoardGameApp.Services.Core;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.BoardGame;
    using BoardGameApp.Web.ViewModels.Reservation;
    using BoardGameApp.Web.ViewModels.Ticket;
    using Microsoft.AspNetCore.Mvc;

    public class TicketController : BaseController
    {
        private readonly ITicketService ticketService;

        public TicketController(ITicketService ticketService)
        {
            this.ticketService = ticketService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<TicketIndexViewModel> activeTickets= await
                ticketService.GetAllTickets();

            return View(activeTickets);
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
    }
}
