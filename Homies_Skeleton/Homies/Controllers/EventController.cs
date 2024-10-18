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
    public class EventController(HomiesDbContext _context) : Controller // USE PRIMARY CONSTRUCTOR for the dependency injection
    {
        private readonly HomiesDbContext context = _context;

        [HttpGet]
        public async Task<IActionResult> All()
        {

            var model = await context.Events
                .Where(s => s.IsDeleted == false)
                .Select(s => new EventInfoViewModel()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Start = s.Start.ToString(DateTimeFormat),
                    Organiser = s.Organiser.UserName ?? string.Empty,
                    Type = s.Type.Name
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
