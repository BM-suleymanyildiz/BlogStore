using BlogStore.EntityLayer.Entities;

namespace BlogStore.DataAccessLayer.Abstract
{
    public interface IAppUserDal : IGenericDal<AppUser>
    {
        // AppUser'a özel veritabanı işlemleri buraya eklenebilir
    }
} 