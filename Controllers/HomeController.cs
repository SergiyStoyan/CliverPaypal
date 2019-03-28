using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CliverPaypal.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CliverPaypal.Controllers
{
    public class HomeController : Controller
    {
        //public HomeController(IMemoryCache cache)
        //{
        //    this.cache = cache;
        //}
        //private IMemoryCache cache;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Payments()
        {
            return View();
        }

        public IActionResult Pay()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Pay(string amount, string currency)
        {
            //cache.Set("", paypal);
            return paypal.BeginPay(this, amount, currency);
            //RedirectResult redirectResult = new RedirectResult(model.Subject.Url, true);
            //return redirectResult;
        }

        static HomeController()
        {
            //var config = ConfigManager.Instance.GetProperties();
            //var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            //var apiContext = new APIContext(accessToken);
            paypal = new Cliver.Paypal("AZc59y7XwWar0eqdPZnv2Taxlw_JtFoKrYQ8O2k-yM4uwp_aEgp4kmzEBLZZFRhxRFifKQZn9fs5CTcs", "ELLnf5hvTMVIJUVTyLTdNlh_U-iOubZs0K0l3Rxjob8eC4ICRT0Z90YKL0vEPxqkalhR1Fn68S5Ns_AX", false);
        }
       readonly static Cliver.Paypal paypal ;
        
        public IActionResult Pay2()
        {
            string result = HttpContext.Request.Query["result"];
            switch (result)
            {
                case "cancel":
                    ViewData["Message"] = "cancel";
                    break;
                case "success":
                    ViewData["Message"] = "success";
                    break;
                default:
                    ViewData["Message"] = "Unknown state: " + HttpContext.Request.Query;
                    break;
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
