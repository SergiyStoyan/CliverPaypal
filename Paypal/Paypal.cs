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
using PayPal.Api;

namespace Cliver.Paypal
{
    public class StorageOptions
    {
        public String StorageConnectionString { get; set; }
        public String AccountName { get; set; }
        public String AccountKey { get; set; }
        public String DefaultEndpointsProtocol { get; set; }
        public String EndpointSuffix { get; set; }

        public StorageOptions() { }
    }

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

        public Paypal()
        {
        }
        readonly APIContext apiContext = Configuration.GetAPIContext();
        //string baseUrl;
        //RedirectUrls redirectUrls;

        public IActionResult BeginPayment(Controller controller, string invoiceNumber, Amount amount, ItemList itemList, string intent = "sale")
        {
            string paymentGuid = Convert.ToString((new Random()).Next(100000));
            Payer payer = new Payer() { payment_method = "paypal" };
            string baseUrl = controller.Request.Scheme + "://" + controller.Request.Host + controller.Request.Path + "?" + QueryKeys.PaymentGuid + "=" + paymentGuid;
            RedirectUrls redirectUrls = new RedirectUrls()
            {
                cancel_url = baseUrl + "&" + QueryKeys.PaymentState + "=" + QueryValues.States.Canceled,
                return_url = baseUrl + "&" + QueryKeys.PaymentState + "=" + QueryValues.States.Created
            };
            List<Transaction> transactions = new List<Transaction>{
                new Transaction()
                {
                    description = "Transaction description.",
                    invoice_number = invoiceNumber,
                    amount = amount,
                    item_list = itemList
                }
            };
            Payment payment = new Payment()
            {
                intent = intent,
                payer = payer,
                transactions = transactions,
                redirect_urls = redirectUrls
            };
            //try
            //{
                Payment createdPayment = payment.Create(apiContext);
                string redirectUrl = createdPayment.links.Where(x => x.rel.ToLower().Trim().Equals("approval_url")).Select(x => x.href).FirstOrDefault();
            //}
            //catch(PayPal.PaymentsException e)
            //{
            //    e.Details
            //}
            if (redirectUrl == null)
                throw new Exception("No approval_url found in the response.");
            controller.HttpContext.Session.SetString(SessionKeys.PaymentId(paymentGuid), createdPayment.id);
            return controller.Redirect(redirectUrl);
        }

        public class QueryKeys
        {
            public const string PaymentState = "paymentState";
            public const string PaymentGuid = "paymentGuid";
            public const string PayerId = "PayerID";
        }

        public class QueryValues
        {
            public class States
            {
                public const string Created = "created";
                public const string Canceled = "canceled";
            }
        }

        public class SessionKeys
        {
            static public string PaymentId(string paymentGuid)
            {
                return "PaymentId" + paymentGuid;
            }
            //public const string Payments = "Payments";
            //public class Payment
            //{
            //    public const string Guid = "Guid";
            //}
        }

        //public enum PaymentResults
        //{
        //    Success,
        //    Canceled,
        //    Failed,
        //}

        public bool IsPaymentBegun(Controller controller)
        {
            return ((string)controller.HttpContext.Request.Query[QueryKeys.PaymentState]) != null;
        }

        public Payment CompletePayment(Controller controller)
        {
            //try
            //{
            string state = controller.HttpContext.Request.Query[QueryKeys.PaymentState];
            switch (state)
            {
                case QueryValues.States.Canceled:
                    return null;
                case QueryValues.States.Created:
                    break;
                default:
                    throw new Exception("Unknown payment result: " + controller.HttpContext.Request.Query);
            }

            string paymentGuid = controller.Request.Query[QueryKeys.PaymentGuid];
            string paymentId = controller.HttpContext.Session.GetString(SessionKeys.PaymentId(paymentGuid));
            string payerId = controller.Request.Query[QueryKeys.PayerId];
            PaymentExecution paymentExecution = new PaymentExecution { payer_id = payerId };
            Payment payment = new Payment() { id = paymentId };
            Payment executedPayment = payment.Execute(apiContext, paymentExecution);
            return executedPayment;
            //}
            //catch(Exception e)
            //{
            //    return PaymentResults.Failed;
            //}
        }
    }
}