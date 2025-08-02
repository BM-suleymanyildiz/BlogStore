using BlogStore.BusinessLayer.Abstract;
using BlogStore.BusinessLayer.Concrete;
using BlogStore.DataAccessLayer.Abstract;
using BlogStore.DataAccessLayer.Context;
using BlogStore.DataAccessLayer.EntityFramework;
using BlogStore.DataAccessLayer.Repositories;
using BlogStore.EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;


namespace BlogStore.BusinessLayer.Container
{
    public static class ServiceRegistration
    {
        public static void AddBusinessLayerServices(this IServiceCollection services)
        {
            // Business Layer Services
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<ICommentService, CommentManager>();
            services.AddScoped<IArticleService, ArticleManager>();
            services.AddScoped<IAppUserService, AppUserManager>();
            services.AddScoped<IToxicDetectionService, ToxicDetectionService>();

            // HttpClient for Hugging Face API
            services.AddHttpClient();

            // Data Access Layer Services
            services.AddScoped<ICategoryDal, EfCategoryDal>();
            services.AddScoped<ICommentDal, EfCommentDal>();
            services.AddScoped<IArticleDal, EfArticleDal>();
            services.AddScoped<IAppUserDal, AppUserRepository>();

            // Database Context
            services.AddDbContext<BlogContext>();

            // Identity Services
            services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<BlogContext>();


        }
    }
} 