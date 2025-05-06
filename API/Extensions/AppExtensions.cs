using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Persistence;
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

            services.AddAutoMapper(
                typeof(Profiles.GrpcProfiles).Assembly,
                typeof(Application.Profiles.DtoProfiles).Assembly
            );

            services.AddScoped<ISymbolRepo, SymbolRepo>();
            services.AddScoped<ISymbolService, SymbolService>();

            return services;
        }
    }
}