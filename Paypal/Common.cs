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
    public static class Common
    {
        //public static string FormatJsonString(string json)
        //{
        //    if (string.IsNullOrEmpty(json))
        //    {
        //        return string.Empty;
        //    }

        //    if (json.StartsWith("["))
        //    {
        //        // Hack to get around issue with the older Newtonsoft library
        //        // not handling a JSON array that contains no outer element.
        //        json = "{\"list\":" + json + "}";
        //        var formattedText = JObject.Parse(json).ToString(Formatting.Indented);
        //        formattedText = formattedText.Substring(13, formattedText.Length - 14).Replace("\n  ", "\n");
        //        return formattedText;
        //    }
        //    return JObject.Parse(json).ToString(Formatting.Indented);
        //}

        public static string GetRandomInvoiceNumber()
        {
            return new Random().Next(999999).ToString();
        }
    }
}