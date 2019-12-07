using System;
using System.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using TeddyBlazor.Areas.Identity;
using TeddyBlazor.Data;
using TeddyBlazor.Services;
using TeddyBlazor.ViewModels;
using TeddyBlazor.ViewModels.ClassDetail;

namespace TeddyBlazor
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private string connectionString;
        private string psqlConnection;
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
            psqlConnection = PostgresUrlParser.PsqlConnection(Configuration["DATABASE_URL"]);
            getDbConnection = () => new NpgsqlConnection(connectionString);


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
            services.AddDefaultIdentity<IdentityUser>(
                options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequiredLength = 0;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            services.AddTransient<Func<IDbConnection>>(c => getDbConnection);
            services.AddTransient<Func<string>>(c => () => psqlConnection);
            services.AddTransient<StudentListViewModel>();
            services.AddTransient<StudentDetailViewModel>();
            services.AddTransient<INewNoteViewModel, StudentNoteViewModel>();
            services.AddTransient<IStudentRepository, StudentRepository>();
            services.AddTransient<IClassRepository, ClassRepository>();
            services.AddTransient<ICourseRepository, CourseRepository>();
            services.AddTransient<ClassListViewModel>();
            services.AddTransient<ClassDetailViewModel>();
            services.AddTransient<SeatingChartViewModel>();
            services.AddTransient<ClassDetailCourseListViewModel>();

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var dbConnectionValidator = new DbConnectionValidator(getDbConnection);
            dbConnectionValidator.validateConnection();

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapControllers();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
