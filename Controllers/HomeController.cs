using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CliverPaypal.Models;
using Microsoft.Extensions.Caching.Memory;
using PayPal.Api;

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
            if (paypal.IsPaymentBegun(this))
            {
                Payment p = paypal.CompletePayment(this);
                if(p!=null)
                    ViewData["Message"] = p.ConvertToJson();
                else
                    ViewData["Message"] = "canceled";
            }
            return View();
        }

        [HttpPost]
        public IActionResult Pay(string total, string currency)
        {
            //cache.Set("", paypal);
            Amount amount = new Amount
            {
                currency = currency,
                // Total must be equal to sum of shipping, tax and subtotal.
                total = total,
                details = new Details()
                {
                    tax = "0",
                    shipping = "0",
                    subtotal = total
                }
            };
            ItemList itemList = new ItemList() { items = new List<Item>() };
            itemList.items.Add(new Item()
            {
                name = "Test Item Name",
                currency = currency,
                price = total,
                quantity = "1",
                sku = "test sku"
            });
            return paypal.BeginPayment(this, Cliver.Paypal.Common.GetRandomInvoiceNumber(), amount, itemList);
        }

        static HomeController()
        {
            //paypal = new Cliver.Paypal("AZc59y7XwWar0eqdPZnv2Taxlw_JtFoKrYQ8O2k-yM4uwp_aEgp4kmzEBLZZFRhxRFifKQZn9fs5CTcs", "ELLnf5hvTMVIJUVTyLTdNlh_U-iOubZs0K0l3Rxjob8eC4ICRT0Z90YKL0vEPxqkalhR1Fn68S5Ns_AX", false);
            paypal = new Cliver.Paypal.Paypal();
        }
       readonly static Cliver.Paypal.Paypal paypal ;
        
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
