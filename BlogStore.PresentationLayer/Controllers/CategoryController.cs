using BlogStore.BusinessLayer.Abstract;
using BlogStore.EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BlogStore.PresentationLayer.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IArticleService _articleService;
        private readonly UserManager<AppUser> _userManager;

        public CategoryController(
            ICategoryService categoryService,
            IArticleService articleService,
            UserManager<AppUser> userManager)
        {
            _categoryService = categoryService;
            _articleService = articleService;
            _userManager = userManager;
        }

        [AllowAnonymous] // Herkes erişebilir
        public IActionResult CategoryList()
        {
            // Tüm kategorileri getir
            var categories = _categoryService.TGetAll() ?? new List<Category>();
            return View("PublicCategoryList", categories);
        }

        [Authorize] // Sadece giriş yapmış kullanıcılar erişebilir
        public async Task<IActionResult> MyCategories()
        {
            // Mevcut kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            // Kullanıcının makalelerinin kategorilerini getir
            var userArticles = _articleService.TGetAll()?.Where(a => a.AppUserId == currentUser.Id).ToList() ?? new List<Article>();
            var userCategoryIds = userArticles.Select(a => a.CategoryId).Distinct().ToList();
            
            var categories = _categoryService.TGetAll()?.Where(c => userCategoryIds.Contains(c.CategoryId)).ToList() ?? new List<Category>();
            
            return View("CategoryList", categories);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryService.TInsert(category);
                return RedirectToAction("CategoryList");
            }
            return View(category);
        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            // Mevcut kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            // Kullanıcının bu kategoriyi kullanıp kullanmadığını kontrol et
            var userArticles = _articleService.TGetAll()?.Where(a => a.AppUserId == currentUser.Id && a.CategoryId == id).ToList() ?? new List<Article>();
            
            if (userArticles.Any())
            {
                // Kullanıcı bu kategoriyi kullanıyorsa silme
                TempData["ErrorMessage"] = "Bu kategoriyi kullandığınız makaleler var. Önce makaleleri silin.";
                return RedirectToAction("CategoryList");
            }
            
            _categoryService.TDelete(id);
            return RedirectToAction("CategoryList");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id)
        {
            // Mevcut kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            // Kullanıcının bu kategoriyi kullanıp kullanmadığını kontrol et
            var userArticles = _articleService.TGetAll()?.Where(a => a.AppUserId == currentUser.Id && a.CategoryId == id).ToList() ?? new List<Article>();
            
            if (!userArticles.Any())
            {
                return RedirectToAction("CategoryList");
            }
            
            var value = _categoryService.TGetById(id);
            return View(value);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryService.TUpdate(category);
                return RedirectToAction("CategoryList");
            }
            return View(category);
        }
    }
}

