using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using BlogStore.BusinessLayer.Abstract;
using BlogStore.EntityLayer.Entities;
using BlogStore.PresentationLayer.Models;
using System.Security.Claims;

namespace BlogStore.PresentationLayer.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar erişebilir
    public class AdminController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly ICommentService _commentService;
        private readonly IAppUserService _appUserService;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(
            IArticleService articleService,
            ICategoryService categoryService,
            ICommentService commentService,
            IAppUserService appUserService,
            UserManager<AppUser> userManager)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _commentService = commentService;
            _appUserService = appUserService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Dashboard";
            
            // Mevcut kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }


            
            // Kullanıcının makalelerini filtrele
            var userArticles = _articleService.TGetAll()?.Where(a => a.AppUserId == currentUser.Id).ToList() ?? new List<Article>();
            
            // Kullanıcının makalelerine gelen yorumları filtrele
            var userComments = _commentService.TGetAll()?.Where(c => userArticles.Any(a => a.ArticleId == c.ArticleId)).ToList() ?? new List<Comment>();
            
            // Bu ayın başlangıç ve bitiş tarihlerini hesapla
            var currentDate = DateTime.Now;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            // Bu ay oluşturulan makaleleri filtrele
            var thisMonthArticles = userArticles.Where(a => a.CreatedDate >= startOfMonth && a.CreatedDate <= endOfMonth).ToList();

            // En çok yorum alan makaleyi bul
            var mostCommentedArticle = userArticles
                .Select(article => new
                {
                    Article = article,
                    CommentCount = userComments.Count(c => c.ArticleId == article.ArticleId)
                })
                .OrderByDescending(x => x.CommentCount)
                .FirstOrDefault();

            // Dashboard için gerekli verileri hazırla (kullanıcıya özel)
            var dashboardData = new DashboardViewModel
            {
                TotalArticles = userArticles.Count,
                TotalCategories = _categoryService.TGetAll()?.Count ?? 0, // Kategoriler genel
                TotalComments = userComments.Count,
                TotalUsers = 1, // Kullanıcı sadece kendini görür
                RecentArticles = userArticles.Take(3).ToList(),
                RecentComments = userComments.Take(5).ToList(),
                PopularArticleTitle = mostCommentedArticle?.Article.Title ?? "Makale Yok",
                PopularArticleCommentCount = mostCommentedArticle?.CommentCount ?? 0
            };
            
            return View(dashboardData);
        }



        public async Task<IActionResult> Profile()
        {
            ViewData["Title"] = "Profilim";
            
            // Mevcut kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            return View(currentUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(AppUser model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Kullanıcı bilgilerini güncelle
            currentUser.Name = model.Name;
            currentUser.Surname = model.Surname;
            currentUser.UserName = model.UserName;
            currentUser.Email = model.Email;

            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profil bilgileriniz başarıyla güncellendi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Profil güncellenirken bir hata oluştu.";
            }

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                TempData["ErrorMessage"] = "Tüm alanları doldurun.";
                return RedirectToAction("Profile");
            }

            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "Yeni şifreler eşleşmiyor.";
                return RedirectToAction("Profile");
            }

            var result = await _userManager.ChangePasswordAsync(currentUser, currentPassword, newPassword);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Şifre değiştirilirken bir hata oluştu. Mevcut şifrenizi kontrol edin.";
            }

            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> CommentList()
        {
            ViewData["Title"] = "Yorumlarım";
            
            // Mevcut kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            // Kullanıcının kendi yaptığı yorumları getir (makale bilgileriyle birlikte)
            var comments = _commentService.GetCommentsByUser(currentUser.Id);
            
            return View(comments);
        }

        public async Task<IActionResult> DeleteComment(int id)
        {
            // Mevcut kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            var comment = _commentService.TGetById(id);
            
            // Kullanıcı sadece kendi yaptığı yorumları silebilir
            if (comment != null && comment.AppUserId == currentUser.Id)
            {
                _commentService.TDelete(id);
                TempData["SuccessMessage"] = "Yorum başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Bu yorumu silme yetkiniz yok.";
            }
            
            return RedirectToAction("CommentList");
        }

        public async Task<IActionResult> ArticleList()
        {
            ViewData["Title"] = "Makalelerim";
            
            // Mevcut kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            // Kullanıcının makalelerini kategori bilgileriyle birlikte getir
            var articles = _articleService.TGetArticlesWithCategories()?.Where(a => a.AppUserId == currentUser.Id).ToList() ?? new List<Article>();
            
            return View(articles);
        }

        public IActionResult ArticleCreate()
        {
            ViewData["Title"] = "Yeni Makale";
            
            // Kategorileri getir
            LoadCategories();
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArticleCreate(Article article)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Mevcut kullanıcıyı al
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser == null)
                    {
                        return RedirectToAction("Login", "Account");
                    }

                    // Makale başlığı kontrolü
                    if (string.IsNullOrWhiteSpace(article.Title) || article.Title.Length < 3 || article.Title.Length > 200)
                    {
                        TempData["ErrorMessage"] = "Makale başlığı 3-200 karakter arasında olmalıdır.";
                        LoadCategories();
                        return View(article);
                    }

                    // Makale açıklaması kontrolü
                    if (string.IsNullOrWhiteSpace(article.Description))
                    {
                        TempData["ErrorMessage"] = "Makale açıklaması boş olamaz.";
                        LoadCategories();
                        return View(article);
                    }

                    article.AppUserId = currentUser.Id;
                    article.CreatedDate = DateTime.Now;
                    
                    _articleService.TInsert(article);
                    TempData["SuccessMessage"] = "Makale başarıyla oluşturuldu.";
                    
                    return RedirectToAction("ArticleList");
                }
                else
                {
                    TempData["ErrorMessage"] = "Form verilerinde hata var. Lütfen kontrol edin.";
                }
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Makale oluşturulurken hata oluştu: {ex.Message}";
            }
            
            // Hata durumunda kategorileri tekrar yükle
            LoadCategories();
            
            return View(article);
        }

        private void LoadCategories()
        {
            var categories = _categoryService.TGetAll()?.ToList() ?? new List<Category>();
            ViewBag.Categories = categories;
        }

        public async Task<IActionResult> ArticleDelete(int id)
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
                TempData["SuccessMessage"] = "Makale başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Bu makaleyi silme yetkiniz yok.";
            }
            
            return RedirectToAction("ArticleList");
        }

        public async Task<IActionResult> ArticleEdit(int id)
        {
            // Mevcut kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            var article = _articleService.TGetById(id);
            
            // Kullanıcı sadece kendi makalelerini düzenleyebilir
            if (article != null && article.AppUserId == currentUser.Id)
            {
                LoadCategories();
                ViewData["Title"] = "Makale Düzenle";
                return View(article);
            }
            else
            {
                TempData["ErrorMessage"] = "Bu makaleyi düzenleme yetkiniz yok.";
                return RedirectToAction("ArticleList");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArticleEdit(Article article)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Mevcut kullanıcıyı al
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser == null)
                    {
                        return RedirectToAction("Login", "Account");
                    }

                    // Makale başlığı kontrolü
                    if (string.IsNullOrWhiteSpace(article.Title) || article.Title.Length < 3 || article.Title.Length > 200)
                    {
                        TempData["ErrorMessage"] = "Makale başlığı 3-200 karakter arasında olmalıdır.";
                        LoadCategories();
                        return View(article);
                    }

                    // Makale açıklaması kontrolü
                    if (string.IsNullOrWhiteSpace(article.Description))
                    {
                        TempData["ErrorMessage"] = "Makale açıklaması boş olamaz.";
                        LoadCategories();
                        return View(article);
                    }

                    // Kullanıcı sadece kendi makalelerini düzenleyebilir
                    var existingArticle = _articleService.TGetById(article.ArticleId);
                    if (existingArticle != null && existingArticle.AppUserId == currentUser.Id)
                    {
                        // Mevcut makaleyi güncelle (yeni entity oluşturma)
                        existingArticle.Title = article.Title;
                        existingArticle.Description = article.Description;
                        existingArticle.CategoryId = article.CategoryId;
                        existingArticle.ImageUrl = article.ImageUrl;
                        // AppUserId ve CreatedDate değişmeyecek
                        
                        _articleService.TUpdate(existingArticle);
                        TempData["SuccessMessage"] = "Makale başarıyla güncellendi.";
                        
                        return RedirectToAction("ArticleList");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Bu makaleyi düzenleme yetkiniz yok.";
                        return RedirectToAction("ArticleList");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Form verilerinde hata var. Lütfen kontrol edin.";
                }
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Makale güncellenirken hata oluştu: {ex.Message}";
            }
            
            // Hata durumunda kategorileri tekrar yükle
            LoadCategories();
            
            return View(article);
        }

        public async Task<IActionResult> CategoryList()
        {
            ViewData["Title"] = "Kategorilerim";
            
            // Mevcut kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            // Tüm kategorileri getir
            var allCategories = _categoryService.TGetAll()?.ToList() ?? new List<Category>();
            
            // Kullanıcının makalelerini getir
            var userArticles = _articleService.TGetAll()?.Where(a => a.AppUserId == currentUser.Id).ToList() ?? new List<Article>();
            
            // Her kategori için makale sayısını hesapla
            var articleCounts = new Dictionary<int, int>();
            foreach (var category in allCategories)
            {
                var count = userArticles.Count(a => a.CategoryId == category.CategoryId);
                articleCounts[category.CategoryId] = count;
            }
            
            ViewBag.ArticleCounts = articleCounts;
            ViewBag.UserArticles = userArticles;
            
            return View(allCategories);
        }

        public IActionResult CategoryCreate()
        {
            ViewData["Title"] = "Yeni Kategori";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CategoryCreate(Category category)
        {
            try
            {
                // Articles property'sini null olarak ayarla
                category.Articles = null;
                
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrWhiteSpace(category.CategoryName))
                    {
                        TempData["ErrorMessage"] = "Kategori adı boş olamaz.";
                        return View(category);
                    }

                    // Kategori adı uzunluğunu kontrol et
                    if (category.CategoryName.Length < 2 || category.CategoryName.Length > 100)
                    {
                        TempData["ErrorMessage"] = "Kategori adı 2-100 karakter arasında olmalıdır.";
                        return View(category);
                    }

                    _categoryService.TInsert(category);
                    return RedirectToAction("CategoryList");
                }
                else
                {
                    TempData["ErrorMessage"] = "Form verilerinde hata var. Lütfen kontrol edin.";
                }
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Kategori oluşturulurken hata oluştu: {ex.Message}";
            }
            
            return View(category);
        }

        public async Task<IActionResult> CategoryDelete(int id)
        {
            // Mevcut kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            // Kullanıcının bu kategoriyi kullandığı makaleleri kontrol et
            var userArticles = _articleService.TGetAll()?.Where(a => a.AppUserId == currentUser.Id && a.CategoryId == id).ToList() ?? new List<Article>();
            
            if (userArticles.Any())
            {
                TempData["ErrorMessage"] = "Bu kategoriyi kullandığınız makaleler var. Önce makaleleri silin veya kategorilerini değiştirin.";
                return RedirectToAction("CategoryList");
            }
            
            _categoryService.TDelete(id);
            TempData["SuccessMessage"] = "Kategori başarıyla silindi.";
            return RedirectToAction("CategoryList");
        }

        public async Task<IActionResult> CategoryEdit(int id)
        {
            // Mevcut kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            // Kullanıcının bu kategoriyi kullandığı makaleleri kontrol et
            var userArticles = _articleService.TGetAll()?.Where(a => a.AppUserId == currentUser.Id && a.CategoryId == id).ToList() ?? new List<Article>();
            
            if (!userArticles.Any())
            {
                TempData["ErrorMessage"] = "Bu kategoriyi kullanmadığınız için düzenleyemezsiniz.";
                return RedirectToAction("CategoryList");
            }
            
            var category = _categoryService.TGetById(id);
            if (category == null)
            {
                return RedirectToAction("CategoryList");
            }
            
            ViewData["Title"] = "Kategori Düzenle";
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CategoryEdit(Category category)
        {
            try
            {
                // Articles property'sini null olarak ayarla
                category.Articles = null;
                
                if (ModelState.IsValid)
                {
                    _categoryService.TUpdate(category);
                    TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
                    return RedirectToAction("CategoryList");
                }
                else
                {
                    TempData["ErrorMessage"] = "Form verilerinde hata var. Lütfen kontrol edin.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Kategori güncellenirken hata oluştu: {ex.Message}";
            }
            
            return View(category);
        }
    }
} 