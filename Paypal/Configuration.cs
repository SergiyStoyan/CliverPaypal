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
    public static class Configuration
    {
        static Configuration()
        {
            //var config = GetConfig();
            //ClientId = config["clientId"];
            //ClientSecret = config["clientSecret"];
            ClientId = "AZc59y7XwWar0eqdPZnv2Taxlw_JtFoKrYQ8O2k-yM4uwp_aEgp4kmzEBLZZFRhxRFifKQZn9fs5CTcs";
            ClientSecret = "ELLnf5hvTMVIJUVTyLTdNlh_U-iOubZs0K0l3Rxjob8eC4ICRT0Z90YKL0vEPxqkalhR1Fn68S5Ns_AX";
            SandboxMode = true;
        }
        public readonly static string ClientId;
        public readonly static string ClientSecret;
        public readonly static bool SandboxMode = true;

        public static Dictionary<string, string> GetConfig()
        {
            return ConfigManager.Instance.GetProperties();
            //Configuration.GetSection("Paypal")
        }

        private static string GetAccessToken()
        {
            // ###AccessToken
            // Retrieve the access token from
            // OAuthTokenCredential by passing in
            // ClientID and ClientSecret
            // It is not mandatory to generate Access Token on a per call basis.
            // Typically the access token can be generated once and
            // reused within the expiry window                
            return new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();
        }

        public static APIContext GetAPIContext(string accessToken = null)
        {
            // ### Api Context
            // Pass in a `APIContext` object to authenticate 
            // the call and to send a unique request id 
            // (that ensures idempotency). The SDK generates
            // a request id if you do not pass one explicitly. 
            var apiContext = new APIContext(accessToken ==null ? GetAccessToken() : accessToken);
            apiContext.Config = GetConfig();

            // Use this variant if you want to pass in a request id  
            // that is meaningful in your application, ideally 
            // a order id.
            // String requestId = Long.toString(System.nanoTime();
            // APIContext apiContext = new APIContext(GetAccessToken(), requestId ));

            return apiContext;
        }
    }
}