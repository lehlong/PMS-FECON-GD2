using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace SMO.Service
{
    public class RecaptchaValidationService
    {
        private const string API_URL = "https://www.google.com/recaptcha/api/siteverify";
        private readonly string _secretKey;

        public RecaptchaValidationService(string secretKey)
        {
            _secretKey = secretKey;
        }

        public bool Validate(string response, string ip)
        {
            if (!string.IsNullOrWhiteSpace(response))
            {
                using (var client = new WebClient())
                {
                    var result = client.DownloadString($"{API_URL}?secret={_secretKey}&response={response}&remoteip={ip}");
                    return ParseValidationResult(result);
                }
            }

            return false;
        }

        private bool ParseValidationResult(string validationResult) => (bool)JObject.Parse(validationResult).SelectToken("success");
    }
}