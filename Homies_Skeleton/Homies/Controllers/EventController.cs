using Homies.Data;
using Homies.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using static Homies.Validation.Constants;
using Type = Homies.Data.Type;


namespace Homies.Controllers
{
    [Authorize]
    public class EventController(HomiesDbContext _context) : Controller // Using the primary constructor for the dependency injection
    {
        private readonly HomiesDbContext context = _context;

        [HttpGet]
        public async Task<IActionResult> All()
        {

            var model = await context.Events
                .Where(e => e.IsDeleted == false)
                .Select(e => new EventInfoViewModel()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Start = e.Start.ToString(DateTimeFormat),
                    Organiser = e.Organiser.UserName ?? string.Empty,
                    Type = e.Type.Name
                })
                .AsNoTracking()
                .ToListAsync();


            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new EventViewModel();

            model.Types = await GetAllTypes();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(EventViewModel model)
        {

            if (!ModelState.IsValid)
            {
                model.Types = await GetAllTypes();
                return View(model);
            }

            DateTime startDateAndTime;

            if (DateTime.TryParseExact(model.Start, DateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out startDateAndTime) == false)
            {
                ModelState.AddModelError(nameof(model.Start), "Invalid start date and time format!");
                return View(model);
            }

            DateTime endDateAndTime;

            if (DateTime.TryParseExact(model.End, DateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out endDateAndTime) == false)
            {
                ModelState.AddModelError(nameof(model.End), "Invalid end date and time format!");
                return View(model);
            }

            model.Types = await GetAllTypes();


            Event newEvent = new Event
            {
                Name = model.Name,
                Description = model.Description,
                OrganiserId = GetCurrentUserId() ?? string.Empty,
                CreatedOn = DateTime.Now,
                Start = startDateAndTime,
                End = endDateAndTime,
                TypeId = model.TypeId,
                IsDeleted = false
            };

            await context.Events.AddAsync(newEvent);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {

            string currentUserId = GetCurrentUserId() ?? string.Empty;

            var model = await context.Events
                .Where(e => e.IsDeleted == false)
                .Where(e => e.EventsParticipants.Any(ep => ep.HelperId == currentUserId))
                .Select(e => new EventInfoViewModel()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Start = e.Start.ToString(DateTimeFormat),
                    Organiser = e.Organiser.UserName ?? string.Empty,
                    Type = e.Type.Name
                })
                .AsNoTracking()
                .ToListAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {

            Event? eventToAdd = await context.Events
                .Where(e => e.Id == id)
                .Include(e => e.EventsParticipants)
                .FirstOrDefaultAsync();


            if (eventToAdd == null || eventToAdd.IsDeleted)
            {
                throw new ArgumentException("Invalid Id");
            }

            string currentUserId = GetCurrentUserId() ?? string.Empty;

            if (eventToAdd.EventsParticipants.Any(ep => ep.HelperId == currentUserId))
            {
                return RedirectToAction(nameof(All)); // If a User tries to add an already added Event to his seminars, they will be redirected to Event/All
            }

            eventToAdd.EventsParticipants.Add(new EventParticipant()
            {
                HelperId = currentUserId,
                EventId = eventToAdd.Id
            });

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Joined));
        }

        [HttpPost]
        public async Task<IActionResult> Leave(int id)
        {
            Event? eventToUnsubscribeFrom = await context.Events
                .Where(e => e.Id == id)
                .Include(e => e.EventsParticipants)
                .FirstOrDefaultAsync();

            if (eventToUnsubscribeFrom == null || eventToUnsubscribeFrom.IsDeleted)
            {
                throw new ArgumentException("Id is not valid.");
            }

            string currentUserId = GetCurrentUserId() ?? string.Empty;
            EventParticipant? eventParticipant = eventToUnsubscribeFrom.EventsParticipants.FirstOrDefault(sp => sp.HelperId == currentUserId);

            if (eventParticipant != null)
            {
                eventToUnsubscribeFrom.EventsParticipants.Remove(eventParticipant);
                await context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var currentUserId = GetCurrentUserId();

            var IsOrganiser = await context.Events
                .AnyAsync(e => e.OrganiserId == currentUserId && e.Id == id);

            var isDeleted = await context.Events.AnyAsync(g => g.Id == id && g.IsDeleted == true);

            if (!IsOrganiser || isDeleted) // check if the user is not the organizer of the event he wants to edit or the event was deleted already
            {
                return RedirectToAction(nameof(All)); // If yes ----> Redirects the user to the Event/All page 
            }

            var model = await context.Events
                .Where(e => e.Id == id)
                .AsNoTracking()
                .Select(e => new EventViewModel
                {
                    Name = e.Name,
                    Description = e.Description,
                    CreatedOn = e.CreatedOn.ToString(DateTimeFormat),
                    Start = e.Start.ToString(DateTimeFormat),
                    End = e.End.ToString(DateTimeFormat),
                    TypeId = e.TypeId,
                })
                .FirstOrDefaultAsync();


            model.Types = await GetAllTypes();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EventViewModel model, int id)
        {

            if (!ModelState.IsValid)
            {
                model.Types = await GetAllTypes();
                return View(model);
            }

            if (DateTime.TryParseExact(model.Start, AcceptedDateTimeFormats, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime startDateAndTime) == false)
            {
                ModelState.AddModelError(nameof(model.Start), "Invalid start date and time format!");
                model.Types = await GetAllTypes();
                return View(model);
            }

            if (DateTime.TryParseExact(model.End, AcceptedDateTimeFormats, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime endDateAndTime) == false)
            {
                ModelState.AddModelError(nameof(model.End), "Invalid end date and time format!");
                model.Types = await GetAllTypes();
                return View(model);
            }

            Event? eventToEdit = await context.Events.FindAsync(id);

            if (eventToEdit == null || eventToEdit.IsDeleted)
            {
                throw new ArgumentException("Invalid Id");
            }

            eventToEdit.Name = model.Name;
            eventToEdit.Description = model.Description;
            eventToEdit.CreatedOn = DateTime.Now;
            eventToEdit.Start = startDateAndTime;
            eventToEdit.End = endDateAndTime;
            eventToEdit.TypeId = model.TypeId;

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await context.Events
                .Where(e => e.Id == id)
                .Where(e => e.IsDeleted == false)
                .AsNoTracking()
                .Select(e => new EventDetailsViewModel
                {
                    Name = e.Name,
                    Start = e.Start.ToString(DateTimeFormat),
                    End = e.End.ToString(DateTimeFormat),
                    CreatedOn = e.CreatedOn.ToString(DateTimeFormat),
                    Description = e.Description,
                    Type = e.Type.Name,
                    Organiser = e.Organiser.UserName
                })
                .FirstOrDefaultAsync();

            return View(model);
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task<List<Type>> GetAllTypes()
        {
            return await context.Types.AsNoTracking().ToListAsync();
        }

    }
}
