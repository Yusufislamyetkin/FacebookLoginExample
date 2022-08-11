using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookLoginExample.Models.Authentication.Contexts;
using FacebookLoginExample.Models.Authentication.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FacebookLoginExample
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ExampleContext>(x => x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<AppUser, AppRole>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequireLowercase = false;
                x.Password.RequiredLength = 3;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<ExampleContext>();
            #region Üçüncü Taraf Kimlik Doðrulamada Cookie Kullanmak Mecburi Deðildir!
            services.ConfigureApplicationCookie(x =>
            {
                x.LoginPath = new PathString("/user/login");
                x.ExpireTimeSpan = TimeSpan.FromMinutes(1);
                x.SlidingExpiration = true;
                x.Cookie = new CookieBuilder
                {
                    HttpOnly = true,
                    Name = "ExampleCookie",
                    SameSite = SameSiteMode.Lax,
                    SecurePolicy = CookieSecurePolicy.Always
                };
            });
            #endregion

            services.AddAuthentication().AddFacebook(x =>
            {
                x.AppId = Configuration["FacebookAppId"];
                x.AppSecret = Configuration["FacebookAppSecret"];
                x.CallbackPath = new PathString("/User/Hata");
            });

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
