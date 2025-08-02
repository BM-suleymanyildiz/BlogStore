using Microsoft.AspNetCore.Mvc;
using BlogStore.BusinessLayer.Abstract;
using BlogStore.EntityLayer.Entities;

namespace BlogStore.PresentationLayer.ViewComponents
{
    public class ArticleDetailFooterComponent : ViewComponent
    {
        private readonly IArticleService _articleService;

        public ArticleDetailFooterComponent(IArticleService articleService)
        {
            _articleService = articleService;
        }

        public IViewComponentResult Invoke()
        {
            // Son eklenen 3 makaleyi al
            var recentArticles = _articleService.TGetAll()
                .OrderByDescending(x => x.CreatedDate)
                .Take(3)
                .ToList();

            return View(recentArticles);
        }
    }
} 