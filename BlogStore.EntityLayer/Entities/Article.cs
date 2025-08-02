using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BlogStore.EntityLayer.Entities
{
    public class Article
    {
        public int ArticleId { get; set; }
        
        [Required(ErrorMessage = "Makale başlığı zorunludur.")]
        [StringLength(200, ErrorMessage = "Makale başlığı en fazla 200 karakter olabilir.")]
        [Display(Name = "Makale Başlığı")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Makale içeriği zorunludur.")]
        [Display(Name = "Makale İçeriği")]
        public string Description { get; set; }
        
        [Display(Name = "Resim URL")]
        public string? ImageUrl { get; set; }
        
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public List<Comment>? Comments { get; set; }
    }
}
