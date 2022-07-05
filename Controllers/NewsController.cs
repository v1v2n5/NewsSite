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

namespace NewsSite.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> manager;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly int PageSize = 6;

        public NewsController(ApplicationDbContext context, UserManager<User> manager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this.manager = manager;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("All");
        }

        /*
        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult AdminList()
        {
            return View(_context.News.ToList());
        }

        */

        /*
        [HttpGet]
        public async Task<IActionResult> List(string id, string category)
        {
            if (int.TryParse(id, out int Id))
            {
                //Получение новости
                return View(new DetailNewsViewModel()
                {
                    CurrentNews = await _context.News.FirstAsync(x => x.Id == Id),
                    RelatedNews = _context.News.Where(x => x.Id != Id).Take(4),

                });
            }
            else
            {
                //Фильтр по категориям
                var news = _context.News
                    .Include(x => x.Author)
                    .Include(x => x.Category)
                    .Include(x => x.Tags)
                    .ThenInclude(x => x.Tag)
                    .Where(x => x.Category.Name == category)
                    .Take(6);
                return View("All", news);
            }
        }
        */
        [HttpGet]
        public async Task<IActionResult> Post(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            /*
            var news = await _context.News.FindAsync(id);
            
            _context.Entry(news).Reference(x => x.Author).Load();
            _context.Entry(news).Reference(x => x.Category).Load();
            _context.Entry(news).Collection(x => x.Tags).Load();
            */
            var news = await _context.News
                .Include(x => x.Author)
                .Include(x => x.Category)
                .Include(x => x.Comments)
                .ThenInclude(x => x.User)
                .Include(x => x.Tags)
                .ThenInclude(x => x.Tag)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        [HttpGet]
        public async Task<IActionResult> All(int page = 1, string category = null, string tag = null)
        {
            int pageNumber = page;

            IQueryable<News> news = _context.News
                .Include(x => x.Author)
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .ThenInclude(x => x.Tag);
            if (!string.IsNullOrEmpty(category))
            {
                news = news.Where(x => x.Category.Name == category);
            }
            if (!string.IsNullOrEmpty(tag)) 
            {
                news = news.Where(x => x.Tags.Any(x => x.Tag.Name == tag));
            }
            int count = await news.CountAsync();
            news = news
                .OrderByDescending(x => x.Time)
                .ThenByDescending(x => x.Id)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize);
            
            return View(new NewsListViewModel()
            {
                News = news,
                PageViewModel = new(
                    count,
                    pageNumber,
                    PageSize),
                Tag = tag,
                Category = category
            });
        }

        [HttpPost]
        public async Task<IActionResult> Comment(int? NewsId, Comment comment)
        {
            if (NewsId == null)
                return NotFound();

            News news = await _context.News.FindAsync(NewsId);
            if (news == null)
                return NotFound();

            comment.User = await manager.GetUserAsync(HttpContext.User);
            comment.Time = DateTime.Now;
            comment.News = news;
            
            _context.Set<Comment>().Add(comment);
            _context.SaveChanges();
            return RedirectToRoute("", new {area = "", controller = "News", action = nameof(Post), Id = NewsId });
        }

        /*
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
        */
        /*
        [HttpGet]
        [Authorize(Roles = "admin, author")]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories.Select(x => x), nameof(Category.Id), nameof(Category.Name));
            return View(new EditNewsViewModel
            {

            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, author")]
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
        [Authorize(Roles = "admin, author")]
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
            ViewBag.Categories = new SelectList(_context.Categories.Select(x => x), nameof(Category.Id), nameof(Category.Name));
            return View(news);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Time,Title,Content")] News news)
        {
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
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News.FindAsync(id);
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        */
        /*
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
        */
    }
}
