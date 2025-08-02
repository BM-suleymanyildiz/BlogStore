using BlogStore.EntityLayer.Entities;

namespace BlogStore.PresentationLayer.Models
{
    public class DashboardViewModel
    {
        public int TotalArticles { get; set; }
        public int TotalCategories { get; set; }
        public int TotalComments { get; set; }
        public int TotalUsers { get; set; }
        public string PopularArticleTitle { get; set; }
        public int PopularArticleCommentCount { get; set; }
        public List<Article> RecentArticles { get; set; } = new List<Article>();
        public List<Comment> RecentComments { get; set; } = new List<Comment>();
    }
} 