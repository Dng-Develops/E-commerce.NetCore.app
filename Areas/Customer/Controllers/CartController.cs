using E_commerce_app.Data;
using E_commerce_app.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace E_commerce_app.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(ApplicationDbContext db, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity; // Get currenly login user
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCart = _db.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product) // Add shopping cart and include product
            };
            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _db.ApplicationUsers.FirstOrDefault(i => i.Id == claim.Value);
            foreach (var item in ShoppingCartVM.ListCart)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += (item.Count * item.Product.Price);
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()  // This action is more or less the same e mail confirmation as Registeration uses
        { 
            var claimsIdentity = (ClaimsIdentity)User.Identity; 
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _db.ApplicationUsers.FirstOrDefault(i => i.Id == claim.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Confirmation email is empty");
            }
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Confirm Your Email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            ModelState.AddModelError(string.Empty, "Send Email Confirmation Code");
            return RedirectToAction("Success");
        }
        public IActionResult Success()
        {
                return View();
        }
        public IActionResult Add(int cartId)
        {
            var cart = _db.ShoppingCarts.FirstOrDefault(i => i.Id == cartId);
            cart.Count += 1;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Decrease(int cartId)
        {
            var cart = _db.ShoppingCarts.FirstOrDefault(i => i.Id == cartId);
            if (cart.Count == 1) // If product count is already 1 dont decrease instead remove form shoppinc cart
            {
                var count = _db.ShoppingCarts.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
                _db.ShoppingCarts.Remove(cart);
                _db.SaveChanges();
                HttpContext.Session.SetInt32(Other.ssShoppingCart, count - 1); // Decrease from session
            }
            else
            {
                cart.Count -= 1;
                _db.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
            var cart = _db.ShoppingCarts.FirstOrDefault(i => i.Id == cartId);

            var count = _db.ShoppingCarts.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
            _db.ShoppingCarts.Remove(cart);
            _db.SaveChanges();
            HttpContext.Session.SetInt32(Other.ssShoppingCart, count - 1); // Remove from session


            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary() 
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity; // Get currenly login user
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCart = _db.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product) // Add shopping cart and include product
            };
            foreach (var item in ShoppingCartVM.ListCart)
            {
                item.Price = item.Product.Price;
                ShoppingCartVM.OrderHeader.OrderTotal += (item.Count * item.Product.Price);
            }
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Summary(ShoppingCartVM model)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity; 
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM.ListCart = _db.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product);
            ShoppingCartVM.OrderHeader.OrderStatus = Other.Purchase_Inprocess;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            _db.OrderHeaders.Add(ShoppingCartVM.OrderHeader);
            _db.SaveChanges();
            foreach (var item in ShoppingCartVM.ListCart)
            {
                item.Price = item.Product.Price;
                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductId = item.ProductId,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = item.Price,
                    Count = item.Count,
                };
                ShoppingCartVM.OrderHeader.OrderTotal += item.Count * item.Product.Price;
                model.OrderHeader.OrderTotal += item.Count * item.Product.Price;
                _db.OrderDetails.Add(orderDetails);
            }
            _db.ShoppingCarts.RemoveRange(ShoppingCartVM.ListCart); // Reset Cart / Remove items form shoppingcart after ordering process..
            _db.SaveChanges();
            HttpContext.Session.SetInt32(Other.ssShoppingCart, 0); // Clear the session data
            return RedirectToAction("OrderReceived"); 
        }

        public IActionResult OrderReceived()
        {
            return View();
        }
    }
}
