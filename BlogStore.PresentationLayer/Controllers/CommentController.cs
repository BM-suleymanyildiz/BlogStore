using BlogStore.BusinessLayer.Abstract;
using BlogStore.EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BlogStore.PresentationLayer.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar erişebilir
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IArticleService _articleService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IToxicDetectionService _toxicDetectionService;

        public CommentController(
            ICommentService commentService, 
            IArticleService articleService,
            UserManager<AppUser> userManager,
            IToxicDetectionService toxicDetectionService)
        {
            _commentService = commentService;
            _articleService = articleService;
            _userManager = userManager;
            _toxicDetectionService = toxicDetectionService;
        }

        public async Task<IActionResult> CommentList()
        {
            // Mevcut kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            // Kullanıcının makalelerine gelen yorumları getir
            var userArticles = _articleService.TGetAll()?.Where(a => a.AppUserId == currentUser.Id).ToList() ?? new List<Article>();
            var userArticleIds = userArticles.Select(a => a.ArticleId).ToList();
            
            var comments = _commentService.TGetAll()?.Where(c => userArticleIds.Contains(c.ArticleId)).ToList() ?? new List<Comment>();
            return View(comments);
        }

        [HttpGet]
        public IActionResult CreateComment()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(Comment comment)
        {
            // Toxic detection kontrolü
            var isToxic = await _toxicDetectionService.IsToxicCommentAsync(comment.CommentDetail);
            
            if (isToxic)
            {
                TempData["ErrorMessage"] = "⚠️ UYARI: Bu yorum uygun değil! Lütfen daha saygılı ve yapıcı bir dil kullanın. Yorumunuz yayınlanmamıştır.";
                return RedirectToAction("CommentList");
            }

            comment.CommentDate = DateTime.Now;
            comment.IsValid = false;
            
            // Yorum ekleme
            var isAdded = await _commentService.TInsertAsync(comment);
            
            if (isAdded)
            {
                TempData["SuccessMessage"] = "Yorum başarıyla eklendi! ✅";
            }
            else
            {
                TempData["ErrorMessage"] = "Yorumunuzu daha düzgün bir dille yazmanızı rica ediyoruz. Lütfen saygılı bir dil kullanın. ⚠️";
            }
            
            return RedirectToAction("CommentList");
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
            
            // Kullanıcı sadece kendi makalelerine gelen yorumları silebilir
            if (comment != null)
            {
                var userArticles = _articleService.TGetAll()?.Where(a => a.AppUserId == currentUser.Id && a.ArticleId == comment.ArticleId).ToList() ?? new List<Article>();
                if (userArticles.Any())
                {
                    _commentService.TDelete(id);
                }
            }
            
            return RedirectToAction("CommentList");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateComment(int id)
        {
            // Mevcut kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            var comment = _commentService.TGetById(id);
            
            // Kullanıcı sadece kendi makalelerine gelen yorumları düzenleyebilir
            if (comment != null)
            {
                var userArticles = _articleService.TGetAll()?.Where(a => a.AppUserId == currentUser.Id && a.ArticleId == comment.ArticleId).ToList() ?? new List<Article>();
                if (userArticles.Any())
                {
                    return View(comment);
                }
            }
            
            return RedirectToAction("CommentList");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateComment(Comment comment)
        {
            // Toxic detection kontrolü
            var isToxic = await _toxicDetectionService.IsToxicCommentAsync(comment.CommentDetail);
            
            if (isToxic)
            {
                TempData["ErrorMessage"] = "⚠️ UYARI: Bu yorum uygun değil! Lütfen daha saygılı ve yapıcı bir dil kullanın. Yorumunuz güncellenmemiştir.";
                return RedirectToAction("CommentList");
            }

            comment.CommentDate = DateTime.Now;
            comment.IsValid = false;
            _commentService.TUpdate(comment);
            return RedirectToAction("CommentList");
        }

        [HttpPost]
        public async Task<IActionResult> AddCommentAjax([FromBody] Comment comment)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    // Toxic detection kontrolü
                    var isToxic = await _toxicDetectionService.IsToxicCommentAsync(comment.CommentDetail);
                    
                    if (isToxic)
                    {
                        return Json(new { 
                            success = false, 
                            message = "⚠️ UYARI: Bu yorum uygun değil! Lütfen daha saygılı ve yapıcı bir dil kullanın. Yorumunuz yayınlanmamıştır." 
                        });
                    }

                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    comment.AppUserId = user.Id;
                    comment.UserNameSurname = user.Name + " " + user.Surname;
                    comment.CommentDate = DateTime.Now;
                    comment.IsValid = true; // Giriş yapmış kullanıcıların yorumları direkt onaylı
                    
                    // Yorum ekleme
                    var isAdded = await _commentService.TInsertAsync(comment);
                    
                    if (isAdded)
                    {
                        return Json(new { success = true, message = "Yorum başarıyla eklendi! ✅" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Yorumunuzu daha düzgün bir dille yazmanızı rica ediyoruz. Lütfen saygılı bir dil kullanın. ⚠️" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Yorum eklenirken bir hata oluştu. Lütfen daha sonra tekrar deneyin." });
                }
            }
            return Json(new { success = false, message = "Giriş yapmanız gerekiyor." });
        }

        [HttpPost]
        public async Task<IActionResult> AddReplyAjax([FromBody] Comment reply)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    // Toxic detection kontrolü
                    var isToxic = await _toxicDetectionService.IsToxicCommentAsync(reply.CommentDetail);
                    
                    if (isToxic)
                    {
                        return Json(new { 
                            success = false, 
                            message = "⚠️ UYARI: Bu yanıt uygun değil! Lütfen daha saygılı ve yapıcı bir dil kullanın. Yanıtınız yayınlanmamıştır." 
                        });
                    }

                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    reply.AppUserId = user.Id;
                    reply.UserNameSurname = user.Name + " " + user.Surname;
                    reply.CommentDate = DateTime.Now;
                    reply.IsValid = true;
                    
                    // Yanıt ekleme
                    var isAdded = await _commentService.TInsertAsync(reply);
                    
                    if (isAdded)
                    {
                        return Json(new { success = true, message = "Yanıtınız başarıyla eklendi! ✅" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Yanıtınızı daha düzgün bir dille yazmanızı rica ediyoruz. Lütfen saygılı bir dil kullanın. ⚠️" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Yanıt eklenirken bir hata oluştu. Lütfen daha sonra tekrar deneyin." });
                }
            }
            return Json(new { success = false, message = "Giriş yapmanız gerekiyor." });
        }
    }
}
