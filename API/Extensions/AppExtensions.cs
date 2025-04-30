using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class AppExtensions
    {
        public static IServiceCollection AddApplicationExtensions(
            this IServiceCollection services, IConfiguration config
        ) {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    config.GetConnectionString("DefaultConnection")
                )
            );

            return services;
        }
    }
}