using Data.Interfaces;
using Data.Servicios;
using Data;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Extension
{
    public static class ClsServicioAplicacionExtension
    {
        public static IServiceCollection AgregarServiciosAplicacion(this IServiceCollection services, IConfiguration config)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Ingresar Bearer [espacio] token \r\n\r\n " +
                                  "Ejemplo: Bearer ejoyasdf23413425324553453245",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
            });
            var conecctionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(conecctionString));
            services.AddCors();
            services.AddScoped<ITokenServicio, ClsTokenServicio>();

            return services;
        }
    }
}
