using BlogStore.BusinessLayer.Abstract;
using BlogStore.DataAccessLayer.Abstract;
using BlogStore.EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogStore.BusinessLayer.Concrete
{
    public class CommentManager : ICommentService
    {
        private readonly ICommentDal _commentDal;

        public CommentManager(ICommentDal commentDal)
        {
            _commentDal = commentDal;
        }
        public void TDelete(int id)
        {
            _commentDal.Delete(id);
        }

        public List<Comment> TGetAll()
        {
            return _commentDal.GetAll();
        }

        public Comment TGetById(int id)
        {
            return _commentDal.GetById(id);
        }

        public List<Comment> TGetCommentsByArticle(int id)
        {
            return _commentDal.GetCommentsByArticle(id);
        }

        public List<Comment> GetCommentsByUser(string userId)
        {
            return _commentDal.GetCommentsByUser(userId);
        }

        public List<Comment> GetCommentsWithReplies(int articleId)
        {
            var allComments = _commentDal.GetCommentsWithUserInfo(articleId);
            
            // Ana yorumları al (ParentCommentId null olanlar)
            var mainComments = allComments.Where(c => c.ParentCommentId == null).ToList();
            
            // Her ana yoruma yanıtlarını ekle
            foreach (var comment in mainComments)
            {
                comment.Replies = allComments.Where(c => c.ParentCommentId == comment.CommentId).ToList();
            }
            
            return mainComments;
        }

        public async Task<bool> TInsertAsync(Comment entity)
        {
            // Yorumu direkt veritabanına ekle
            _commentDal.Insert(entity);
            return true; // Başarıyla eklendi
        }

        public void TInsert(Comment entity)
        {
            // Senkron versiyon
            _commentDal.Insert(entity);
        }

        public void TUpdate(Comment entity)
        {
            _commentDal.Update(entity);
        }


    }
}
