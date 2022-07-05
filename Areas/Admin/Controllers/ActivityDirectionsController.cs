using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsSite.Data;
using NewsSite.Models;
using NewsSite.Models.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NewsSite.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [Area("Admin")]
    public class ActivityDirectionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ActivityDirectionsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View("List", _context.ActivityDirections.ToList());
        }

        // GET: ActivityDirectionsController/Create
        public ActionResult Create()
        {
            ViewBag.Action = "Create";
            return View("Edit");
        }

        // POST: ActivityDirectionsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("PreviewContent,Title,Content,PreviewImage")] ActivityDirectionsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var activityDirections = new ActivityDirections
                {
                    Title = model.Title,
                    PreviewContent = model.PreviewContent,
                    Content = model.Content,
                    PreviewImage = UploadedFile(model)
                };


                _context.Add(activityDirections);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            return View("Edit",model);
        }

        // GET: ActivityDirectionsController/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            ViewBag.Action = "Edit";
            if (id == null)
            {
                return NotFound();
            }

            var model = await _context.ActivityDirections.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: ActivityDirectionsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("Id,PreviewContent,Title,Content")] ActivityDirections model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(new ActivityDirectionsViewModel(model));
        }

        // GET: ActivityDirectionsController/Delete/5
        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var ActivityDirections = _context.ActivityDirections
                .FirstOrDefault(m => m.Id == id);
            if (ActivityDirections == null)
            {
                return NotFound();
            }

            return View(ActivityDirections);
        }

        // POST: ActivityDirectionsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            var ActivityDirections =  _context.ActivityDirections.Find(id);
            _context.ActivityDirections.Remove(ActivityDirections);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult List()
        {
            return View(_context.ActivityDirections.Select(x => x));
        }

        public async Task<IActionResult> Post(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _context.ActivityDirections.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        private string UploadedFile(ActivityDirectionsViewModel model)
        {
            string uniqueFileName = null;
            if (model.PreviewImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");

                uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.PreviewImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                model.PreviewImage.CopyTo(fileStream);
            }
            return uniqueFileName;
        }

        private bool ModelExists(int id)
        {
            return _context.ActivityDirections.Any(e => e.Id == id);
        }
    }
}
