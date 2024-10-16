using GameZone.Data;
using GameZone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

            model.Genres = await context.Genres.AsNoTracking().ToListAsync();
                
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Add(GameViewModel model)
		{
			return View();
		}


		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			var model = new GameViewModel();

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(GameViewModel model)
		{
			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> MyZone()
		{
			return View(new List<GameInfoViewModel>());
		}

		[HttpGet]
		public async Task<IActionResult> AddToMyZone(int id)
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> StrikeOut(int id)
		{
			return View();
		}


		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> Delete(int id)
		{
			return View();
		}

	}
}
