using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {

        // db config
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        // // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        // builder.Services.AddEndpointsApiExplorer();
        // builder.Services.AddSwaggerGen();
        services.AddCors();
        services.AddScoped<ITokenServices, TokenService>();
        services.AddScoped<IUserRepository,UserRepository>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());//assembles DTO appusers & member DTO check >helpers> AutopMapperProfiles
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));// gets the Cloudinarysettings from appsettings.jsonn
        services.AddScoped<IphotoService, PhotoService>(); //StartupBase photouplpoad & DeleteBehavior methods

        return services;
    }
}