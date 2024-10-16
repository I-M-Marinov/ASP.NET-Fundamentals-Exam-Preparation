using System.Globalization;
using System.Security.Claims;
using GameZone.Data;
using GameZone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static GameZone.Constants.ValidationConstants;

namespace GameZone.Controllers
{
	[Authorize]
	public class GameController : Controller
	{
		private readonly GameZoneDbContext context;

		public GameController(GameZoneDbContext _context)
		{
			context = _context;
		}

		[HttpGet]
		public async Task<IActionResult> All()
		{

			var model = await context.Games
                .Where(g => g.IsDeleted == false)
                .Select(g => new GameInfoViewModel()
                {
					Id = g.Id,
					Genre = g.Genre.Name,
					ImageUrl = g.ImageUrl,
					Publisher = g.Publisher.UserName ?? string.Empty,
					ReleasedOn = g.ReleasedOn.ToString(GameReleasedOnFormat),
					Title = g.Title
                })
                .AsNoTracking()
                .ToListAsync();


			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> Add()
		{
			var model = new GameViewModel();

            model.Genres = await GetGenres();
                
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Add(GameViewModel model)
		{

            if (!ModelState.IsValid)
            {
                model.Genres = await GetGenres();
                return View(model);
            }

            DateTime releasedOn;

            if (DateTime.TryParseExact(model.ReleasedOn, GameReleasedOnFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out releasedOn) == false)
            {
				ModelState.AddModelError(nameof(model.ReleasedOn),"Invalid date format!");
                model.Genres = await GetGenres();
                return View(model);
            }

            Game game = new Game()
            {
				Description = model.Description,
				GenreId = model.GenreId,
				ImageUrl = model.ImageUrl,
				PublisherId = GetCurrentUserId() ?? string.Empty,
				ReleasedOn = releasedOn,
				Title = model.Title
            };

			await context.Games.AddAsync(game);
			await context.SaveChangesAsync();

			return RedirectToAction(nameof(All));
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int id)
        {
            var model = await context.Games
                .Where(g => g.Id == id)
                .Where(g => g.IsDeleted == false)
                .AsNoTracking()
                .Select(g => new GameViewModel()
                {
					Description = g.Description,
					GenreId = g.GenreId,
					ImageUrl = g.ImageUrl,
					ReleasedOn = g.ReleasedOn.ToString(GameReleasedOnFormat),
					Title = g.Title
                })
                .FirstOrDefaultAsync();

            model.Genres = await GetGenres();

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(GameViewModel model, int id)
		{
            if (!ModelState.IsValid)
            {
                model.Genres = await GetGenres();
                return View(model);
            }

            DateTime releasedOn;

            if (DateTime.TryParseExact(model.ReleasedOn, GameReleasedOnFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out releasedOn) == false)
            {
                ModelState.AddModelError(nameof(model.ReleasedOn), "Invalid date format!");
                model.Genres = await GetGenres();
                return View(model);
            }

            Game? entity = await context.Games.FindAsync(id);

            if (entity == null || entity.IsDeleted) 
            {
                throw new ArgumentException("Invalid Id");
            }

            entity.Description = model.Description;
            entity.GenreId = model.GenreId;
            entity.ImageUrl = model.ImageUrl;
            entity.PublisherId = GetCurrentUserId() ?? string.Empty;
            entity.ReleasedOn = releasedOn;
            entity.Title = model.Title;

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

		[HttpGet]
		public async Task<IActionResult> MyZone()
        {
            string currentUserId = GetCurrentUserId() ?? string.Empty;

            var model = await context.Games
                .Where(g => g.IsDeleted == false)
                .Where(g => g.GamersGames.Any(gr => gr.GamerId == currentUserId))
                .Select(g => new GameInfoViewModel()
                {
                    Id = g.Id,
                    Genre = g.Genre.Name,
                    ImageUrl = g.ImageUrl,
                    Publisher = g.Publisher.UserName ?? string.Empty,
                    ReleasedOn = g.ReleasedOn.ToString(GameReleasedOnFormat),
                    Title = g.Title
                })
                .AsNoTracking()
                .ToListAsync();


            return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> AddToMyZone(int id)
        {
            Game? entity = await context.Games
                .Where(g => g.Id == id)
                .Include(g => g.GamersGames)
                .FirstOrDefaultAsync();

            if (entity == null || entity.IsDeleted)
            {
                throw new ArgumentException("Invalid Id");
            }

            string currentUserId = GetCurrentUserId() ?? string.Empty;

            if (entity.GamersGames.Any(gr => gr.GamerId == currentUserId))
            {
                return RedirectToAction(nameof(All)); // If a User tries to add an already added game to their collection, they should be redirected to /Game/All (or just a page refresh)
            }

            entity.GamersGames.Add(new GamerGame()
            {
                GamerId = currentUserId,
                GameId = entity.Id
            });

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(MyZone)); // Upon successful Adding a Game to the User's collection, should be redirected to the /Game/MyZone
        }

        [HttpGet]
		public async Task<IActionResult> StrikeOut(int id)
		{
            Game? entity = await context.Games
                .Where(g => g.Id == id)
                .Include(g => g.GamersGames)
                .FirstOrDefaultAsync();

            if (entity == null || entity.IsDeleted)
            {
                throw new ArgumentException("Invalid Id");
            }

            string currentUserId = GetCurrentUserId() ?? string.Empty;
            GamerGame? gamerGame = entity.GamersGames.FirstOrDefault(gr => gr.GamerId == currentUserId);

            if (gamerGame != null)
            {
                entity.GamersGames.Remove(gamerGame);
                await context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(MyZone));
        }


		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
            var model = await context.Games
                .Where(g => g.Id == id)
                .Where(g => g.IsDeleted == false)
                .AsNoTracking()
                .Select(g => new GameDetailsViewModel()
                {
                    Id = g.Id,
                    Description = g.Description,
                    Genre = g.Genre.Name,
                    ImageUrl = g.ImageUrl,
                    ReleasedOn = g.ReleasedOn.ToString(GameReleasedOnFormat),
                    Title = g.Title,
                    Publisher = g.Publisher.UserName ?? string.Empty
                })
                .FirstOrDefaultAsync();

            return View(model);
        }

		[HttpGet]
		public async Task<IActionResult> Delete(int id)
		{
            var model = await context.Games
                .Where(g => g.Id == id)
                .Where(g => g.IsDeleted == false)
                .AsNoTracking()
                .Select(g => new DeleteGameViewModel()
                {
                    Id = g.Id,
                    Title = g.Title,
                    Publisher = g.Publisher.UserName ?? string.Empty
                })
                .FirstOrDefaultAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(DeleteGameViewModel model)
        {
            Game? game = await context.Games
                .Where(g => g.Id == model.Id)
                .Where(g => g.IsDeleted == false)
                .FirstOrDefaultAsync();

            if (game != null)
            {
                game.IsDeleted = true;
                await context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(All));
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task<List<Genre>> GetGenres()
        {
            return await context.Genres.AsNoTracking().ToListAsync();
        } 
	}
}
