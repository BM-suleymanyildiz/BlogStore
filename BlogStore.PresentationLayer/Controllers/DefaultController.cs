using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BlogStore.BusinessLayer.Abstract;
using BlogStore.EntityLayer.Entities;

namespace BlogStore.PresentationLayer.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<AppUser> _userManager;

        public DefaultController(IArticleService articleService, ICategoryService categoryService, UserManager<AppUser> userManager)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CategoryArticles(int id)
        {
            // Kategori adını al
            var category = _categoryService.TGetById(id);
            if (category == null)
            {
                return RedirectToAction("Index");
            }

            // Bu kategorideki makaleleri kategori bilgisiyle birlikte getir
            var allArticles = _articleService.TGetArticlesWithCategories();
            var articles = allArticles?.Where(a => a.CategoryId == id).ToList() ?? new List<Article>();
            
            ViewBag.CategoryName = category.CategoryName;
            ViewBag.Articles = articles;
            
            return View();
        }

        public IActionResult Authors()
        {
            // Tüm kullanıcıları getir (yazarlar)
            var authors = _userManager.Users.ToList();
            
            ViewBag.Authors = authors;
            ViewBag.Title = "Yazarlar";
            
            return View();
        }

        public IActionResult AuthorArticles(string id)
        {
            // Yazarı bul
            var author = _userManager.FindByIdAsync(id).Result;
            if (author == null)
            {
                return RedirectToAction("Authors");
            }

            // Bu yazara ait makaleleri getir
            var articles = _articleService.TGetArticlesWithCategories()?.Where(a => a.AppUserId == id).ToList() ?? new List<Article>();
            
            ViewBag.AuthorName = $"{author.Name} {author.Surname}";
            ViewBag.AuthorUsername = author.UserName;
            ViewBag.Articles = articles;
            
            return View();
        }
    }
}
