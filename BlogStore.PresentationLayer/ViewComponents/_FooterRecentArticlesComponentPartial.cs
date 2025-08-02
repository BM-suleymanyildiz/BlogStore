using BlogStore.BusinessLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace BlogStore.PresentationLayer.ViewComponents
{
    public class _FooterRecentArticlesComponentPartial : ViewComponent
    {
        private readonly IArticleService _articleService;
        
        public _FooterRecentArticlesComponentPartial(IArticleService articleService)
        {
            _articleService = articleService;
        }
        
        public IViewComponentResult Invoke()
        {
            // Son 3 makaleyi getir (en son oluşturulanlar)
            var recentArticles = _articleService.TGetArticlesWithCategories()?.OrderByDescending(x => x.CreatedDate).Take(3).ToList();
            return View(recentArticles);
        }
    }
} 