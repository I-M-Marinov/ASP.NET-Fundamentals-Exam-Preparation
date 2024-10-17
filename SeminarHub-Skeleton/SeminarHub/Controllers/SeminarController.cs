using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeminarHub.Data;
using SeminarHub.Models;
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using static SeminarHub.ValidationConstants.Constants;


namespace SeminarHub.Controllers
{
    [Authorize]

public class SeminarController(SeminarHubDbContext _context) : Controller // USE PRIMARY CONSTRUCTOR
    {
        private readonly SeminarHubDbContext context = _context;

        [HttpGet]
        public async Task<IActionResult> All()
        {

            var model = await context.Seminars
                .Where(s => s.IsDeleted == false)
                .Select(s => new SeminarInfoViewModel()
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    Category = s.Category.Name,
                    DateAndTime = s.DateAndTime.ToString(DateAndTimeFormat),
                    Organizer = s.Organizer.UserName ?? string.Empty
                })
                .AsNoTracking()
                .ToListAsync();


            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new SeminarViewModel();

            model.Categories = await GetAllCategories();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SeminarViewModel model)
        {

            if (!ModelState.IsValid)
            {
                model.Categories = await GetAllCategories();
                return View(model);
            }

            DateTime dateAndTime;

            if (DateTime.TryParseExact(model.DateAndTime, DateAndTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateAndTime) == false)
            {
                ModelState.AddModelError(nameof(model.DateAndTime), "Invalid date and time format!");
                model.Categories = await GetAllCategories();

                return View(model);
            }

            Seminar seminar = new Seminar
            {
                Topic = model.Topic,
                Lecturer = model.Lecturer,
                Details = model.Details,
                DateAndTime = dateAndTime,
                Duration = model.Duration,
                CategoryId = model.CategoryId,
                OrganizerId = GetCurrentUserId() ?? string.Empty,
                IsDeleted = false
            };

            await context.Seminars.AddAsync(seminar);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            
            string currentUserId = GetCurrentUserId() ?? string.Empty;

            var model = await context.Seminars
                .Where(s => s.IsDeleted == false)
                .Where(s => s.SeminarsParticipants.Any(sp => sp.ParticipantId == currentUserId))
                .Select(s => new SeminarInfoViewModel()
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    Category = s.Category.Name,
                    DateAndTime = s.DateAndTime.ToString(DateAndTimeFormat),
                    Organizer = s.Organizer.UserName ?? string.Empty
                })
                .AsNoTracking()
                .ToListAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {

            Seminar? entity = await context.Seminars
                .Where(s => s.Id == id)
                .Include(s => s.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if (entity == null || entity.IsDeleted)
            {
                throw new ArgumentException("Invalid Id");
            }

            string currentUserId = GetCurrentUserId() ?? string.Empty;

            if (entity.SeminarsParticipants.Any(sp => sp.ParticipantId == currentUserId))
            {
                return RedirectToAction(nameof(All)); // If a User tries to add an already added seminar to his seminars, they should be redirected to /Seminar/All (or just a page refresh)
            }

            entity.SeminarsParticipants.Add(new SeminarParticipant()
            {
                ParticipantId = currentUserId,
                SeminarId = entity.Id
            });

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            return View();
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private string? GetCurrentUserName()
        {
            return User.FindFirstValue(ClaimTypes.Name);
        }

        private async Task<List<Category>> GetAllCategories()
        {
            return await context.Categories.AsNoTracking().ToListAsync();
        }
    }
}
