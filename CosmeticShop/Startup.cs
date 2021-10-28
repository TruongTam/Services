using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CosmeticShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace CosmeticShop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false; // KHÔNG BAO GIỜ ĐỂ LÀ TRUE, LÀM ƠN
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // ----- Phần này để sử dụng Session. Hướng dẫn tại : https://www.c-sharpcorner.com/article/all-about-session-in-asp-net-core/
            services.AddDistributedMemoryCache();
            services.AddSession(p =>
            {
                p.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            // ----- //

            services.AddSingleton<HtmlEncoder>(
    HtmlEncoder.Create(allowedRanges: new[] {
                UnicodeRanges.All
    }));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDbContext<CosmeticShopDbContext>(dbContextOptionBuilder => dbContextOptionBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opt =>
                {
                    opt.LoginPath = "/Admin/Login";
                    opt.AccessDeniedPath = "/Home/AccessDenied";
                    opt.LogoutPath = "/KhachHang/Logout";
                });
            services.AddSingleton<IConfiguration>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            // config for RobotsTXT SEO tool
            /*
            app.UseRobotsTxt(builder =>
               builder
                   .AddSection(section =>
                       section
                           .AddComment("Allow Googlebot")
                           .AddUserAgent("Googlebot")
                           .Allow("/")
                       )
                   .AddSection(section =>
                       section
                           .AddComment("Disallow the rest")
                           .AddUserAgent("*")
                           .AddCrawlDelay(TimeSpan.FromSeconds(10))
                           .Disallow("/")
                       )
            );
            */
            // config for RobotsTXT SEO tool
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
