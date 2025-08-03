namespace BoardGameApp.Areas.Manager.Controllers
{
    using BoardGameApp.Controllers;
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Manager.Interfaces;
    using BoardGameApp.Web.ViewModels.Manager.Reservations;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using static BoardGameApp.GCommon.ApplicationConstants;

    [Area("Manager")]
    [Authorize(Roles = RoleManager)]
    public class ReservationsController : BaseController
    {
        private readonly ITicketReservationService ticketService;
        private readonly IRepository<GameSession> gameSessionRepository;

        public ReservationsController(ITicketReservationService ticketService, IRepository<GameSession> gameSessionRepository)
        {
            this.ticketService = ticketService;
            this.gameSessionRepository = gameSessionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ManageTickets()
        {
            try
            {
                IEnumerable<GameSession> sessions = await gameSessionRepository
                .All()
                .Include(s => s.BoardGame)
                .Where(s => !s.IsDeleted)
                .ToListAsync();

                IEnumerable<GameSessionTicketViewModel> model = sessions
                    .Select(s => new GameSessionTicketViewModel
                    {
                        GameSessionId = s.Id,
                        GameTitle = s.BoardGame.Title,
                        CurrentPlayers = s.CurrentPlayers,
                        MaxPlayers = s.MaxPlayers
                    })
                    .ToList();

                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTickets(Guid gameSessionId, int maxPlayers)
        {
            try
            {
                var success = await ticketService.UpdateMaxPlayersAsync(gameSessionId, maxPlayers);

                if (!success)
                {
                    TempData["Error"] = "Cannot update tickets. Session may already have players or is invalid.";
                }
                else
                {
                    TempData["Success"] = "Tickets updated successfully.";
                }

                return RedirectToAction(nameof(ManageTickets));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index), "Home");
            }
        }
    }
}
