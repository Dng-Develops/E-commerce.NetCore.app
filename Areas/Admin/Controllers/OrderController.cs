using E_commerce_app.Data;
using E_commerce_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace E_commerce_app.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public OrderDetailsVM OrderVM { get; set; }
        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }
    
        public IActionResult Index() // Admin will see all the orders while user sees only his/her orders
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeadersList;
            if (User.IsInRole(Other.Role_Admin))
            {
                orderHeadersList = _db.OrderHeaders.ToList();
            }
            else
            {
                orderHeadersList = _db.OrderHeaders.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.ApplicationUser);
            }
            return View(orderHeadersList);
        }
        public IActionResult InProcess() 
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeadersList;
            if (User.IsInRole(Other.Role_Admin))
            {
                orderHeadersList = _db.OrderHeaders.Where(i => i.OrderStatus == Other.Purchase_Inprocess);
            }
            else
            {
                orderHeadersList = _db.OrderHeaders.Where(i => i.ApplicationUserId == claim.Value && i.OrderStatus == Other.Purchase_Inprocess).Include(i => i.ApplicationUser);
            }
            return View(orderHeadersList);
        }

        public IActionResult Approved()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeadersList;
            if (User.IsInRole(Other.Role_Admin))
            {
                orderHeadersList = _db.OrderHeaders.Where(i => i.OrderStatus == Other.Purchase_Confirmed);
            }
            else
            {
                orderHeadersList = _db.OrderHeaders.Where(i => i.ApplicationUserId == claim.Value && i.OrderStatus == Other.Purchase_Confirmed).Include(i => i.ApplicationUser);
            }
            return View(orderHeadersList);
        }

        public IActionResult Shipped()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeadersList;
            if (User.IsInRole(Other.Role_Admin))
            {
                orderHeadersList = _db.OrderHeaders.Where(i => i.OrderStatus == Other.Purchase_Shipped);
            }
            else
            {
                orderHeadersList = _db.OrderHeaders.Where(i => i.ApplicationUserId == claim.Value && i.OrderStatus == Other.Purchase_Shipped).Include(i => i.ApplicationUser);
            }
            return View(orderHeadersList);
        }

        public IActionResult Details(int id)
        {
            OrderVM = new OrderDetailsVM()
            {
                OrderHeader = _db.OrderHeaders.FirstOrDefault(i => i.Id == id),
                OrderDetails = _db.OrderDetails.Where(x => x.OrderId == id ).Include(x => x.Product)
            };
            return View(OrderVM);
        }

        /* Order State */

        [HttpPost]
        [Authorize(Roles = Other.Role_Admin)]
        public IActionResult ConfirmOrder()
        {
            OrderHeader orderHeader = _db.OrderHeaders.FirstOrDefault(i=>i.Id==OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = Other.Purchase_Confirmed;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = Other.Role_Admin)]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _db.OrderHeaders.FirstOrDefault(i => i.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = Other.Purchase_Shipped;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
