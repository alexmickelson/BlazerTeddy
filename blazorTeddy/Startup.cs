using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using TeddyBlazor.Data;
using TeddyBlazor.Services;
using TeddyBlazor.ViewModels;

namespace TeddyBlazor
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private string connectionString;
        Func<IDbConnection> getDbConnection;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            connectionString = PostgresUrlParser.ParseConnectionString(Configuration["DATABASE_URL"]);
            getDbConnection = () => new NpgsqlConnection(connectionString);
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddTransient<Func<IDbConnection>>(c => getDbConnection);
            services.AddTransient<IStudentRepository, StudentRepository>();
            services.AddTransient<StudentListViewModel>();
            services.AddTransient<StudentDetailViewModel>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            validateDbConnection();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private void validateDbConnection()
        {
            try
            {
                using(var dbConnection = getDbConnection())
                {
                    dbConnection.Execute("select * from student limit 1;");
                }
            }
            catch (Npgsql.NpgsqlException)
            {
                throw new Npgsql.NpgsqlException($"Cannot Connect to database at connection string: '{connectionString}'");
            }
        }
    }
}
