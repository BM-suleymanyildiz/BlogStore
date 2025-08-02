using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BlogStore.EntityLayer.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        
        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Kategori Adı")]
        public string CategoryName { get; set; }
        
        public List<Article>? Articles { get; set; }
    }
}
