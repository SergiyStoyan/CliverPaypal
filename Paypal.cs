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

namespace Cliver
{
    public class Paypal
    {
        //public Paypal This
        //{
        //    get
        //    {
        //        if (_This == null)
        //            _This = new Paypal("AZc59y7XwWar0eqdPZnv2Taxlw_JtFoKrYQ8O2k-yM4uwp_aEgp4kmzEBLZZFRhxRFifKQZn9fs5CTcs", "ELLnf5hvTMVIJUVTyLTdNlh_U-iOubZs0K0l3Rxjob8eC4ICRT0Z90YKL0vEPxqkalhR1Fn68S5Ns_AX", false);
        //        return _This;
        //    }
        //}
        //Paypal _This = null;

        public Paypal(string clientId, string clientSecret, bool live)
        {
            if (live)
                environment = new LiveEnvironment(clientId, clientSecret);
            else
                environment = new SandboxEnvironment(clientId, clientSecret);
            client = new PayPalHttpClient(environment);
        }
        PayPalEnvironment environment;
        PayPalHttpClient client;
        public RedirectUrls RedirectUrls = new RedirectUrls()
        {
            ReturnUrl = "/Home/Pay2?result=success",
            CancelUrl = "/Home/Pay"
        };
        public Payer Payer = new Payer()
        {
            PaymentMethod = "paypal"
        };

        public IActionResult BeginPay(Controller controller, string currency, string itemName, string price, string intent = "sale")
        {
            Payment payment = new Payment()
            {
                Intent = intent,
                Transactions = new List<Transaction>()
                    {
                        new Transaction()
                        {
                            Amount = new Amount()
                            {
                                Total = price,
                                Currency = currency,
                //                Details = new AmountDetails
                //{
                //    Tax = "15",
                //    Shipping = "10",
                //    Subtotal = "75"
                //}
                            },
                            Description = "test description",
            //ItemList = new ItemList
            //{
            //    Items = new List<Item>
            //    {
            //        new Item
            //        {
            //            Name = itemName,
            //            Currency = currency,
            //            Price = price,
            //            Quantity = "1",
            //            //Sku = "sku"
            //        }
            //    }
            //}
                        }
                    },
                RedirectUrls = RedirectUrls,
                Payer = Payer,
            };

            PaymentCreateRequest request = new PaymentCreateRequest();
            request.RequestBody(payment);
            System.Net.HttpStatusCode statusCode;
            try
            {
                BraintreeHttp.HttpResponse response = client.Execute(request).Result;
                statusCode = response.StatusCode;
                payment = response.Result<Payment>();
            }
            catch (BraintreeHttp.HttpException e)
            {
                statusCode = e.StatusCode;
                var debugId = e.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();
                throw new Exception("Request failed! HTTP response code: " + statusCode + ", debug ID: " + debugId, e);
            }

            string redirectUrl = payment.Links.Where(x => x.Rel.Equals("approval_url")).Select(x => x.Href).FirstOrDefault();
            if (redirectUrl == null)
                throw new Exception("No approval_url found in the response.");
            return controller.Redirect(redirectUrl);
        }
    }
}