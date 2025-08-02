using System.Threading.Tasks;

namespace BlogStore.BusinessLayer.Abstract
{
    public interface IToxicDetectionService
    {
        Task<bool> IsToxicCommentAsync(string commentText);
        Task<double> GetToxicityScoreAsync(string commentText);
    }
} 