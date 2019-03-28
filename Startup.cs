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
using PayPal.Core;
using PayPal.v1.Payments;

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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public void Configure1(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            paypal(app, "AZc59y7XwWar0eqdPZnv2Taxlw_JtFoKrYQ8O2k-yM4uwp_aEgp4kmzEBLZZFRhxRFifKQZn9fs5CTcs", "ELLnf5hvTMVIJUVTyLTdNlh_U-iOubZs0K0l3Rxjob8eC4ICRT0Z90YKL0vEPxqkalhR1Fn68S5Ns_AX", true);
        }

        void paypal(IApplicationBuilder app, string clientId, string clientSecret, bool sandbox = false)
        {
            app.Run(async (context) =>
            {
                PayPalEnvironment environment;
                if (sandbox)
                    environment = new SandboxEnvironment(clientId, clientSecret);
                else
                    environment = new LiveEnvironment(clientId, clientSecret);

                var client = new PayPalHttpClient(environment);

                var payment = new Payment()
                {
                    Intent = "sale",
                    Transactions = new List<Transaction>()
                    {
                        new Transaction()
                        {
                            Amount = new Amount()
                            {
                                Total = "10",
                                Currency = "USD"
                            }
                        }
                    },
                    RedirectUrls = new RedirectUrls()
                    {
                        ReturnUrl = "https://www.ReturnUrl.com/",
                        CancelUrl = "https://www.CancelUrl.com"
                    },
                    Payer = new Payer()
                    {
                        PaymentMethod = "paypal"
                    }
                };

                PaymentCreateRequest request = new PaymentCreateRequest();
                request.RequestBody(payment);

                System.Net.HttpStatusCode statusCode;

                try
                {
                    BraintreeHttp.HttpResponse response = await client.Execute(request);
                    statusCode = response.StatusCode;
                    Payment result = response.Result<Payment>();

                    string redirectUrl = null;
                    foreach (LinkDescriptionObject link in result.Links)
                    {
                        if (link.Rel.Equals("approval_url"))
                        {
                            redirectUrl = link.Href;
                            break;
                        }
                    }

                    if (redirectUrl == null)
                        await context.Response.WriteAsync("Failed to find an approval_url in the response!");
                    else
                        await context.Response.WriteAsync("Now <a href=\"" + redirectUrl + "\">go to PayPal to approve the payment</a>.");
                }
                catch (BraintreeHttp.HttpException ex)
                {
                    statusCode = ex.StatusCode;
                    var debugId = ex.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();
                    await context.Response.WriteAsync("Request failed!  HTTP response code was " + statusCode + ", debug ID was " + debugId);
                }
            });
        }
    }
}