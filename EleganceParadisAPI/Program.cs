
using EleganceParadisAPI.Configurations;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace EleganceParadisAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            Infrastructure.Dependencies.ConfigureServices(builder.Configuration, builder.Services);
            //builder.Services.AddDbContext<EleganceParadisContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("EleganceParadisDB")));

            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins("http://eleganceparadis.azurewebsites.net",
                                           "http://localhost:5173/")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                    });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { 
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        }, 
                        new List<string>() 
                    } 
                });
            });

            builder.Services.AddApplicationCoreServices()
                            .AddWebAPIServices()
                            .AddAuthServices(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
            }
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseCors();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
