using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeminarHub.Data;
using SeminarHub.Models;
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using static SeminarHub.ValidationConstants.Constants;
using Microsoft.VisualBasic;
using System;


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
        public async Task<IActionResult> Edit(int id)
        {
            var currentUserId = GetCurrentUserId();

            var IsOrganizer = await context.Seminars
                .AnyAsync(g => g.OrganizerId == currentUserId && g.Id == id);

            var isDeleted = await context.Seminars.AnyAsync(g => g.Id == id && g.IsDeleted == true);

            if (!IsOrganizer || isDeleted) // check if the user is the organizer of the seminar he wants to edit or the seminar was deleted already
            {
                return RedirectToAction(nameof(All)); // If yes ----> Redirects the user to the Seminar/All page 
            }

            var model = await context.Seminars
                .Where(g => g.Id == id)
                .AsNoTracking()
                .Select(s => new SeminarViewModel()
                {
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    Details = s.Details,
                    DateAndTime = s.DateAndTime.ToString(DateAndTimeFormat),
                    Duration = s.Duration,
                    CategoryId = s.CategoryId
                })
                .FirstOrDefaultAsync();


            model.Categories = await GetAllCategories();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SeminarViewModel model, int id)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await GetAllCategories();
                return View(model);
            }

            DateTime dateTime;

            if (DateTime.TryParseExact(model.DateAndTime, DateAndTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTime) == false)
            {
                ModelState.AddModelError(nameof(model.DateAndTime), "Invalid date and time format!");
                model.Categories = await GetAllCategories();
                return View(model);
            }

            Seminar? entity = await context.Seminars.FindAsync(id);

            if (entity == null || entity.IsDeleted)
            {
                throw new ArgumentException("Invalid Id");
            }

            entity.Topic = model.Topic;
            entity.Lecturer = model.Lecturer;
            entity.Details = model.Details;
            entity.DateAndTime = dateTime;
            entity.Duration = model.Duration;
            entity.CategoryId = model.CategoryId;

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await context.Seminars
                .Where(g => g.Id == id)
                .Where(g => g.IsDeleted == false)
                .AsNoTracking()
                .Select(g => new SeminarDetailsViewModel()
                {
                    Id = g.Id,
                    Topic = g.Topic,
                    Lecturer = g.Lecturer,
                    Category = g.Category.Name,
                    DateAndTime = g.DateAndTime.ToString(DateAndTimeFormat),
                    Organizer = g.Organizer.UserName ?? string.Empty,
                    Details = g.Details,
                    Duration = g.Duration
                })
                .FirstOrDefaultAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await context.Seminars
                .Where(s => s.Id == id)
                .Where(s => s.IsDeleted == false)
                .AsNoTracking()
                .Select(s => new SeminarDeleteViewModel()
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    DateAndTime = s.DateAndTime
                })
                .FirstOrDefaultAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(SeminarDeleteViewModel model)
        {
            Seminar? seminarToDelete = await context.Seminars
                .Where(s => s.Id == model.Id)
                .Where(s => s.IsDeleted == false)
                .FirstOrDefaultAsync();

            if (seminarToDelete != null)
            {
                seminarToDelete.IsDeleted = true; // SOFT DELETE // 
                await context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(All));
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task<List<Category>> GetAllCategories()
        {
            return await context.Categories.AsNoTracking().ToListAsync();
        }
    }
}
