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

namespace CliverPaypal
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
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //services.AddMemoryCache();
            services.AddSession();

            //services.Configure<Cliver.Paypal.StorageOptions>(Configuration.GetSection("Paypal"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(/*IConfiguration configuration, */IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            //app.UseHttpContext();//to get Base Url
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    //public class MyHttpContext
    //{
    //    private static IHttpContextAccessor m_httpContextAccessor;

    //    public static HttpContext Current => m_httpContextAccessor.HttpContext;

    //    public static string AppBaseUrl => $"{Current.Request.Scheme}://{Current.Request.Host}{Current.Request.PathBase}";

    //    internal static void Configure(IHttpContextAccessor contextAccessor)
    //    {
    //        m_httpContextAccessor = contextAccessor;
    //    }
    //}

    //public static class HttpContextExtensions
    //{
    //    public static void AddHttpContextAccessor(this IServiceCollection services)
    //    {
    //        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    //    }

    //    public static IApplicationBuilder UseHttpContext(this IApplicationBuilder app)
    //    {
    //        MyHttpContext.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
    //        return app;
    //    }
    //}
}