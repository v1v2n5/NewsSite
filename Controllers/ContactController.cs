using Microsoft.AspNetCore.Mvc;
using NewsSite.Data;
using NewsSite.Models;

namespace NewsSite.Controllers
{
    public class ContactController : Controller
    {
        private ApplicationDbContext dbContext;
        public ContactController(ApplicationDbContext dbContext )
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Message(Contact contact)
        {
            if (ModelState.IsValid)
            {
                dbContext.Contacts.Add(contact);
                dbContext.SaveChanges();
                return RedirectToRoute("", new { area = "", controller = "Home", action = "Index"});
            }
            else
                return View(nameof(Index), contact);
        }
    }
}
