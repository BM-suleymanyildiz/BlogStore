using BlogStore.DataAccessLayer.Abstract;
using BlogStore.DataAccessLayer.Context;
using BlogStore.EntityLayer.Entities;

namespace BlogStore.DataAccessLayer.Repositories
{
    public class AppUserRepository : GenericRepository<AppUser>, IAppUserDal
    {
        public AppUserRepository(BlogContext context) : base(context)
        {
        }
    }
} 