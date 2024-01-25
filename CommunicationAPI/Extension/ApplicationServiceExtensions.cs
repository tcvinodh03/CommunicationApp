using CommunicationAPI.Data;
using CommunicationAPI.Helpers;
using CommunicationAPI.Interface;
using CommunicationAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace CommunicationAPI.Extension
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration config)
        {
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            services.AddCors();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IuserRepo, UserRepo>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<ILikesRepo, LikesRepo>();
            services.AddScoped<IMessageRepo, MessageRepo>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<LogUserActivity>();
            return services;
        }
    }
}
