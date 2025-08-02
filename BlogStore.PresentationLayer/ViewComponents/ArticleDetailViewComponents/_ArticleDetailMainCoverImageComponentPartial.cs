using BlogStore.BusinessLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace BlogStore.PresentationLayer.ViewComponents.ArticleDetailViewComponents
{
    public class _ArticleDetailMainCoverImageComponentPartial : ViewComponent
    {
        private readonly IArticleService _articleService;

        public _ArticleDetailMainCoverImageComponentPartial(IArticleService articleService)
        {
            _articleService = articleService;
        }

        public IViewComponentResult Invoke(int id)
        {
            var article = _articleService.TGetArticlesWithAppUser(id);
            if (article == null)
            {
                return View("Error"); // Hata view'u
            }
            return View(article);
        }
    }
}
