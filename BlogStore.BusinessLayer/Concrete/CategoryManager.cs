using BlogStore.BusinessLayer.Abstract;
using BlogStore.DataAccessLayer.Abstract;
using BlogStore.DataAccessLayer.Dtos;
using BlogStore.EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogStore.BusinessLayer.Concrete
{
    public class CategoryManager : ICategoryService
    {
        private readonly ICategoryDal _categoryDal;
        public CategoryManager(ICategoryDal categoryDal)
        {
            _categoryDal = categoryDal;
        }

        public void TDelete(int id)
        {
            _categoryDal.Delete(id);
        }

        public List<Category> TGetAll()
        {
            return _categoryDal.GetAll();
        }

        public Category TGetById(int id)
        {
            return _categoryDal.GetById(id);
        }

        public List<CategoryWithArticleArticleCountDto> TGetCategoryWithArticleCount()
        {
            return _categoryDal.GetCategoryWithArticleCount();
        }

        public void TInsert(Category entity)
        {
            if (!string.IsNullOrWhiteSpace(entity.CategoryName) && entity.CategoryName.Length >= 2 && entity.CategoryName.Length <= 100)
            {
                _categoryDal.Insert(entity);
            }
            else
            {
                throw new ArgumentException("Kategori adı 2-100 karakter arasında olmalıdır ve boş olamaz.");
            }
        }

        public void TUpdate(Category entity)
        {
            if (!string.IsNullOrWhiteSpace(entity.CategoryName) && entity.CategoryName.Length >= 2 && entity.CategoryName.Length <= 100)
            {
                _categoryDal.Update(entity);
            }
            else
            {
                throw new ArgumentException("Kategori adı 2-100 karakter arasında olmalıdır ve boş olamaz.");
            }
        }
    }
}
