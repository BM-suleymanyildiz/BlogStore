using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using BlogStore.PresentationLayer.Models;
using BlogStore.BusinessLayer.Abstract;
using BlogStore.EntityLayer.Entities;

namespace BlogStore.PresentationLayer.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<AppUser> _userManager;

        public ArticleController(
            IArticleService articleService, 
            ICategoryService categoryService,
            UserManager<AppUser> userManager)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _userManager = userManager;
        }

        public IActionResult ArticleDetail(string id)
        {
            int? decodedId = IdEncoder.DecodeId(id);
            if (decodedId == null)
            {
                return View("Error");
            }
            ViewBag.i = decodedId.Value;
            return View();
        }

        // Admin Actions
        [Authorize]
        public async Task<IActionResult> List()
        {
            ViewData["Title"] = "Makalelerim";
            
            // Mevcut kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            // Sadece kullanıcının makalelerini getir
            var userArticles = _articleService.TGetAll()?.Where(a => a.AppUserId == currentUser.Id).ToList() ?? new List<Article>();
            return View(userArticles);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Yeni Makale";
            ViewBag.Categories = _categoryService.TGetAll();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Article article)
        {
            if (ModelState.IsValid)
            {
                // Mevcut kullanıcıyı al
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    article.AppUserId = currentUser.Id;
                    article.CreatedDate = DateTime.Now;
                    _articleService.TInsert(article);
                    return RedirectToAction("List");
                }
            }
            ViewBag.Categories = _categoryService.TGetAll();
            return View(article);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Makale Düzenle";
            
            // Mevcut kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            var article = _articleService.TGetById(id);
            
            // Kullanıcı sadece kendi makalelerini düzenleyebilir
            if (article == null || article.AppUserId != currentUser.Id)
            {
                return RedirectToAction("List");
            }
            
            ViewBag.Categories = _categoryService.TGetAll();
            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Article article)
        {
            if (ModelState.IsValid)
            {
                // Mevcut kullanıcıyı al
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    // Kullanıcı sadece kendi makalelerini düzenleyebilir
                    var existingArticle = _articleService.TGetById(article.ArticleId);
                    if (existingArticle != null && existingArticle.AppUserId == currentUser.Id)
                    {
                        article.AppUserId = currentUser.Id; // Kullanıcı ID'sini koru
                        _articleService.TUpdate(article);
                        return RedirectToAction("List");
                    }
                }
            }
            ViewBag.Categories = _categoryService.TGetAll();
            return View(article);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            // Mevcut kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            var article = _articleService.TGetById(id);
            
            // Kullanıcı sadece kendi makalelerini silebilir
            if (article != null && article.AppUserId == currentUser.Id)
            {
                _articleService.TDelete(id);
            }
            
            return RedirectToAction("List");
        }
    }
}
