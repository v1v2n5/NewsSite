using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsSite.Data;
using NewsSite.Extentions;
using NewsSite.Models;
using NewsSite.Models.ViewModels;

namespace NewsSite.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [Area("Admin")]
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> manager;
        private readonly IWebHostEnvironment webHostEnvironment;
        //private readonly int PageSize = 6;

        public NewsController(ApplicationDbContext context, UserManager<User> manager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this.manager = manager;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(nameof(AdminList), _context.News.OrderByDescending(x => x.Id).ToList());
        }

        [HttpGet]

        public IActionResult AdminList()
        {
            return View(_context.News.OrderByDescending(x => x.Id).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            _context.Entry(news).Collection(x => x.Tags).Load();
            foreach (var tag in news.Tags)
                _context.Entry(tag).Reference(x => x.Tag).Load();
            _context.Entry(news).Reference(x => x.Category).Load();

            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories.Select(x => x), nameof(Category.Id), nameof(Category.Name));
            return View(new EditNewsViewModel
            {

            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EditNewsViewModel newsViewModel)
        {
            if (ModelState.IsValid)
            {
                var news = new News
                {
                    Time = newsViewModel.Time,
                    Title = newsViewModel.Title,
                    PreviewContent = newsViewModel.PreviewContent,
                    Content = newsViewModel.Content,
                    Author = await manager.GetUserAsync(HttpContext.User)
                };
                if (int.TryParse(newsViewModel.Category, out int Categoty_id))
                {
                    if (!_context.Categories.Any(x => x.Id == Categoty_id))
                    {
                        ModelState.AddModelError(string.Empty, "Не найдено ни одной категории");
                        return View(newsViewModel);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Не найдено ни одной категории");
                    return View(newsViewModel);
                }
                news.Category = _context.Categories.First(x => x.Id == Categoty_id);
                news.PreviewImage = UploadedFile(newsViewModel);

                var tags = from tag in newsViewModel.Tags.Split(',') select tag.Trim().FirstCharToUpper();

                var tags_objects =
                    from tag in tags
                    join tag_object in _context.Tags
                    on tag equals tag_object.Name into tags_context_objects
                    from tag_object in tags_context_objects.DefaultIfEmpty()
                    select tag_object ?? new Tag() { Name = tag };

                news.Tags = tags_objects.Select(x => new NewsTags() { Tag = x }).ToList();
                _context.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(newsViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            _context.Entry(news).Collection(x => x.Tags).Load();
            foreach (var tag in news.Tags)
                _context.Entry(tag).Reference(x => x.Tag).Load();
            _context.Entry(news).Reference(x => x.Category).Load();

            ViewBag.Categories = new SelectList(_context.Categories.Select(x => x), nameof(Category.Id), nameof(Category.Name));
            return View(new EditNewsViewModel(news));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditNewsViewModel newsViewModel)
        {

            if (ModelState.IsValid)
            {

                var news = _context.News.Find(newsViewModel.Id);
                if (news == null)
                {
                    return NotFound();
                }

                _context.Entry(news).Collection(x => x.Tags).Load();
                foreach (var tag in news.Tags)
                    _context.Entry(tag).Reference(x => x.Tag).Load();
                _context.Entry(news).Reference(x => x.Category).Load();

                if (news.Time != newsViewModel.Time)
                    news.Time = newsViewModel.Time;
                if (news.Title != newsViewModel.Title)
                    news.Title = newsViewModel.Title;
                if (news.PreviewContent != newsViewModel.PreviewContent)
                    news.PreviewContent = newsViewModel.PreviewContent;

                if (int.TryParse(newsViewModel.Category, out int Categoty_id))
                {
                    if (!_context.Categories.Any(x => x.Id == Categoty_id))
                    {
                        ModelState.AddModelError(string.Empty, "Не найдено ни одной категории");
                        return View(newsViewModel);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Не найдено ни одной категории");
                    return View(newsViewModel);
                }
                var Category = _context.Categories.First(x => x.Id == Categoty_id);
                if (news.Category != Category)
                    news.Category = Category;
                if (newsViewModel.PreviewImage != null)
                    news.PreviewImage = UploadedFile(newsViewModel);

                var tags = from tag in newsViewModel.Tags.Split(',') select tag.Trim().FirstCharToUpper();

                var tag_context = _context.Tags;

                var tags_objects =
                    from tag in tags
                    join tag_object in tag_context
                    on tag equals tag_object.Name into tags_context_objects
                    from tag_object in tags_context_objects.DefaultIfEmpty()
                    select tag_object ?? new Tag() { Name = tag };


                _context.NewsTags.RemoveRange(news.Tags);
                news.Tags = tags_objects.Distinct().Select(x => new NewsTags() { Tag = x }).ToList();
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

                //Автора не трогаем

                /*
                var news = new News
                {
                    Time = newsViewModel.Time,
                    Title = newsViewModel.Title,
                    PreviewContent = newsViewModel.PreviewContent,
                    Content = newsViewModel.Content,
                    Author = await manager.GetUserAsync(HttpContext.User)
                };
                */
                /*
                if (int.TryParse(newsViewModel.Category, out int Categoty_id))
                {
                    if (!_context.Categories.Any(x => x.Id == Categoty_id))
                    {
                        ModelState.AddModelError(string.Empty, "Не найдено ни одной категории");
                        return View(newsViewModel);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Не найдено ни одной категории");
                    return View(newsViewModel);
                }
                */
                /*
                news.Category = _context.Categories.First(x => x.Id == Categoty_id);
                news.PreviewImage = UploadedFile(newsViewModel);

                var tags = from tag in newsViewModel.Tags.Split(',') select tag.Trim().FirstCharToUpper();

                var tags_objects =
                    from tag in tags
                    join tag_object in _context.Tags
                    on tag equals tag_object.Name into tags_context_objects
                    from tag_object in tags_context_objects.DefaultIfEmpty()
                    select tag_object ?? new Tag() { Name = tag };

                news.Tags = tags_objects.Select(x => new NewsTags() { Tag = x }).ToList();
                _context.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                */
            }
            return View(newsViewModel);



            /*

            if (id != news.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.Id))
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
            return View(news);
            */
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News.FindAsync(id);
            _context.Entry(news).Collection(x => x.Tags).Load();
            foreach (var tag in news.Tags)
                _context.Entry(tag).Reference(x => x.Tag).Load();
            _context.NewsTags.RemoveRange(news.Tags);
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }

        private string UploadedFile(EditNewsViewModel model)
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
    }
}
