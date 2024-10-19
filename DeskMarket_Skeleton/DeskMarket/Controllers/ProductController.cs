using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DeskMarket.Data;
using DeskMarket.Models;
using Microsoft.EntityFrameworkCore;
using static DeskMarket.Validation.Constants;
using System.Globalization;
using System.Security.Claims;
using DeskMarket.Data.Models;


namespace DeskMarket.Controllers
{
    [Authorize]
    public class ProductController(ApplicationDbContext _context) : Controller // Using the primary constructor for the dependency injection
    {

        private readonly ApplicationDbContext context = _context;

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = GetCurrentUserId();


            var model = await context.Products
                .Where(p => p.IsDeleted == false)
                .Select(p => new ProductInfoViewModel
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    IsSeller = p.SellerId == currentUser,
                    HasBought = p.ProductsClients.Any(pc => pc.ClientId == currentUser)
                })
                .AsNoTracking()
                .ToListAsync();


            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new ProductViewModel();

            model.Categories = await GetCategories();
            model.SellerId = GetCurrentUserId();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategories();
                return View(model);
            }

            if (model.Price < ProductPriceMinValue || model.Price > ProductPriceMaxValue)
            {
                ModelState.AddModelError(nameof(model.Price), "The price must be between 1.00 and 3000.00.");
                model.Categories = await GetCategories();
                return View(model);
            }

            if (DateTime.TryParseExact(model.AddedOn, AddedOnFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime addedOn) == false)
            {
                ModelState.AddModelError(nameof(model.AddedOn), "Invalid added on date and time format!");
                model.Categories = await GetCategories();
                return View(model);
            }

            model.Categories = await GetCategories();
            var currentUserId = GetCurrentUserId();


            Product newProduct = new Product()
            {
                ProductName = model.ProductName,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                AddedOn = addedOn,
                CategoryId = model.CategoryId,
                SellerId = currentUserId
            };


            await context.Products.AddAsync(newProduct);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task<List<Category>> GetCategories()
        {
            return await context.Categories.AsNoTracking().ToListAsync();
        }


    }
}
