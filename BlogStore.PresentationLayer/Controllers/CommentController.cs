using Microsoft.AspNetCore.Mvc;
using BlogStore.BusinessLayer.Abstract;
using BlogStore.EntityLayer.Entities;
namespace BlogStore.PresentationLayer.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }
        public IActionResult CommentList()
        {
            var values = _commentService.TGetAll();
            return View(values);
        }
        [HttpGet]

        public IActionResult CreateComment()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateComment(Comment comment)
        {
            _commentService.TInsert(comment);
            return RedirectToAction("CommentList");
        }
    }
}
