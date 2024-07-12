using Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class Dependencies
    {
        public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddDbContext<EleganceParadisContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("EleganceParadisDB")));
            services.AddTransient<IDbConnection>(sp => new SqlConnection(configuration.GetConnectionString("EleganceParadisDB")));
        }
    }
}
