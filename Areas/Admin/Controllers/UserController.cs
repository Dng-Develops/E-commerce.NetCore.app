using E_commerce_app.Data;
using E_commerce_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace E_commerce_app.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Other.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var users = _db.ApplicationUsers.ToList();
            var role = _db.Roles.ToList();
            var userRole = _db.UserRoles.ToList();
            foreach (var item in users)
            {
                var roleId = userRole.FirstOrDefault(i => i.UserId == item.Id).RoleId;
                item.Role = role.FirstOrDefault(x => x.Id == roleId).Name;
            }
            return View(users);
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _db.ApplicationUsers
                .FirstOrDefaultAsync(m => m.Id == id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _db.ApplicationUsers.FindAsync(id);
            _db.ApplicationUsers.Remove(user);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
