
using EleganceParadisAPI.Configurations;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
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

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            builder.Services.AddApplicationCoreServices()
                            .AddWebAPIServices();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
            }
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
