using IotControlService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace IotControlService
{
    public static class ConfigureServices
    {
        public static void SetUpSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "IoT Control API",
                    Version = "v1"
                }
            ));
        }

        public static void SetUpIdentity(this IServiceCollection services, IConfigurationManager config)
        {
            services.AddDbContext<DataContext>(opts =>
                opts.UseNpgsql(config.GetConnectionString("PostgresConnection")));

            services.AddIdentity<AppUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();
        }

        public static void DatabaseMigrate(this IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                if (dbContext.Database.GetPendingMigrations().Any())
                {
                    dbContext.Database.Migrate();
                }
            }
        }

        public static void SetUpSwaggerUI(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "IoT Control API v1");
                options.RoutePrefix = "docs";
            });
        }
    }
}
