using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartSearch.Core.Application.Contracts.Repository.Base;
using SmartSearch.Core.Application.Contracts.UnitOfWork;
using SmartSearch.Infrastructure.IoC.Mapper;
using SmartSearch.Infrastructure.Persistance.Context;
using SmartSearch.Infrastructure.Persistance.Repository.Base;
using SmartSearch.Infrastructure.Persistance.UnitOfWork;
using SmartSearch.Infrastructure.Persistence.Repository.AppUser;
using SmartSearch.Infrastructure.Persistence.Repository.Document;
using SmartSearch.Infrastructure.Persistence.Repository.Video;
using SmartSearch.Modules.DocumentManager.Repository;
using SmartSearch.Modules.DocumentManager.Service;
using SmartSearch.Modules.DocumentManager.Service.Implementation;
using SmartSearch.Modules.UserManager.Repository;
using SmartSearch.Modules.UserManager.Service;
using SmartSearch.Modules.UserManager.Service.Implementation;
using SmartSearch.Modules.VideoManager.Repository;
using SmartSearch.Modules.VideoManager.Service;
using SmartSearch.Modules.VideoManager.Service.Implementation;

namespace SmartSearch.Infrastructure.IoC
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>))
                .AddScoped<IAppUserRepository, AppUserRepository>()
                .AddScoped<IDocumentRepository, DocumentRepository>()
                .AddScoped<IDocumentKeywordRepository, DocumentKeywordRepository>()
                .AddScoped<IVideoRepository, VideoRepository>()
                .AddScoped<IVideoKeywordRepository, VideoKeywordRepository>();
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddScoped<IAppUserService, AppUserService>()
                .AddScoped<IDocumentService, DocumentService>()
                .AddScoped<IDocumentKeywordService, DocumentKeywordService>()
                .AddScoped<IVideoService, VideoService>()
                .AddScoped<IVideoKeywordService, VideoKeywordService>();
        }

        public static IServiceCollection AddMapper(this IServiceCollection services)
        {
            return services.AddAutoMapper(typeof(AutoMapperProfile));
        }
    }
}
