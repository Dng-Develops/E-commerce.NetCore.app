using E_commerce_app.Data;
using E_commerce_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace E_commerce_app.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
           var products =  _db.Products.Where(i => i.IsHome).ToList();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier); // Take the user
            if (claim!=null)
            {
                var count = _db.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).ToList().Count(); // Product count in cart
                HttpContext.Session.SetInt32(Other.ssShoppingCart, count); // Add session
            }
            return View(products);
        }
        public IActionResult Details(int id)
        {
            var product = _db.Products.FirstOrDefault(i => i.Id == id);
            ShoppingCart cart = new ShoppingCart()
            {
                Product = product,
                ProductId = product.Id

            };
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart Scart) // Shopping cart
        {
            Scart.Id = 0;
            if (ModelState.IsValid) 
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity; 
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier); // Take the user
                Scart.ApplicationUserId = claim.Value; // assign user as Shopping cart user    
                ShoppingCart cart = _db.ShoppingCarts.FirstOrDefault( // Find if same cart exists
                    u => u.ApplicationUserId == Scart.ApplicationUserId && u.ProductId == Scart.ProductId);
                if (cart == null) // if same cart exists increase count if not add new cart to db 
                {
                    _db.ShoppingCarts.Add(Scart);
                }
                else
                {
                    cart.Count += Scart.Count; 
                }
                _db.SaveChanges();
                var count = _db.ShoppingCarts.Where(i => i.ApplicationUserId == Scart.ApplicationUserId).ToList().Count();// Customer count
                HttpContext.Session.SetInt32(Other.ssShoppingCart, count);// Add Session
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var product = _db.Products.FirstOrDefault(i => i.Id == Scart.Id);
                ShoppingCart cart = new ShoppingCart()
                {
                    Product = product,
                    ProductId = product.Id,

                };
            }
       
            return View(Scart);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
