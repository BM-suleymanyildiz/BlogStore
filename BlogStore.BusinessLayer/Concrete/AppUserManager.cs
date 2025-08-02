using BlogStore.BusinessLayer.Abstract;
using BlogStore.DataAccessLayer.Abstract;
using BlogStore.EntityLayer.Entities;

namespace BlogStore.BusinessLayer.Concrete
{
    public class AppUserManager : IAppUserService
    {
        private readonly IAppUserDal _appUserDal;

        public AppUserManager(IAppUserDal appUserDal)
        {
            _appUserDal = appUserDal;
        }

        public void TInsert(AppUser entity)
        {
            _appUserDal.Insert(entity);
        }

        public void TDelete(int id)
        {
            _appUserDal.Delete(id);
        }

        public void TUpdate(AppUser entity)
        {
            _appUserDal.Update(entity);
        }

        public List<AppUser> TGetAll()
        {
            return _appUserDal.GetAll();
        }

        public AppUser TGetById(int id)
        {
            return _appUserDal.GetById(id);
        }
    }
} 