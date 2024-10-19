﻿using Microsoft.AspNetCore.Authorization;
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

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            string currentUserId = GetCurrentUserId() ?? string.Empty;

            Product? productToAdd = await context.Products
                .Where(p => p.Id == id)
                .Include(p => p.ProductsClients)
                .FirstOrDefaultAsync();

            bool isDuplicate = await context.ProductsClients
                .AsNoTracking()
                .AnyAsync(pc => pc.ProductId == productToAdd.Id && pc.ClientId == currentUserId);


            if (productToAdd == null || productToAdd.IsDeleted)
            {
                throw new ArgumentException("Id is invalid");
            }

            if (isDuplicate)
            {
                ModelState.AddModelError(string.Empty, "Products cannot be duplicated");
                return RedirectToAction(nameof(Index));
            }


            if (productToAdd.ProductsClients.Any(pc => pc.ClientId == currentUserId))
            {
                return RedirectToAction(nameof(Index)); // If a User tries to add an already added product to his cart, they will be redirected to Product/Index
            }

            productToAdd.ProductsClients.Add(new ProductClient()
            {
                ClientId = currentUserId,
                ProductId = productToAdd.Id
            });


            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Upon successful Adding a Product to the User's Cart, should be redirected to the /Product/Index
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {

            string currentUserId = GetCurrentUserId() ?? string.Empty;

            var model = await context.Products
                .Where(p => p.IsDeleted == false)
                .Where(p => p.ProductsClients.Any(pc => pc.ClientId == currentUserId))
                .Select(p => new ProductInfoViewModel()
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    IsSeller = p.SellerId == currentUserId,
                    HasBought = p.ProductsClients.Any(pc => pc.ClientId == currentUserId)
                })
                .AsNoTracking()
                .ToListAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            Product? productToRemove = await context.Products
                .Where(p => p.Id == id)
                .Include(e => e.ProductsClients)
                .FirstOrDefaultAsync();

            if (productToRemove == null || productToRemove.IsDeleted)
            {
                throw new ArgumentException("Product Id is not valid.");
            }

            string currentUserId = GetCurrentUserId() ?? string.Empty;

            ProductClient? productClient = productToRemove.ProductsClients.FirstOrDefault(pc => pc.ClientId == currentUserId);

            if (productClient != null)
            {
                productToRemove.ProductsClients.Remove(productClient);
                await context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Cart)); // Upon successful Removal of a Product from the User's Cart, should be redirected to the /Product/Cart
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
