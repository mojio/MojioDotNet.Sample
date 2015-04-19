using System;

namespace MojioDotNet.Sample.Cross.Models
{
    public class ApplicationConfiguration
    {
        private string _host;

        public ApplicationConfiguration()
        {
            ApplicationId = "8c67d1a8-835e-4a7d-ba8d-566798f125fb";
            Uri uri = null;
            System.Uri.TryCreate("http://localhost/done", UriKind.RelativeOrAbsolute, out uri);
            RedirectUri = uri;
            Live = true;
            BingMapCredentials = "AiVIP0IcaljvepBb6VRlOFNBLtC8HEbqavrDWAxc5nI2Am2XprFY2rv5PfZb5buw";
        }

        private string _prod = "https://api.moj.io/v1";
        private string _stage = "https://staging.api.moj.io/v1";

        public string Host
        {
#if STAGE
            get { return _stage; }
#else
            get { return _prod; }
#endif
        }

        public string SecretKey
        {
            get
            {
                if (Live)
                {
                    return "424fca03-ad2c-4f25-93b6-5abac0ba3acf";
                }
                else
                {
                    //sandbox key
                    return "ce928bda-78b8-4192-91e5-0a7b193fddf8";
                }
            }
        }

        public string ApplicationId { get; set; }

        public System.Uri RedirectUri { get; set; }

        public bool Live { get; set; }

        public string BingMapCredentials { get; set; }

        public System.Uri AuthorizeUri { get; set; }
    }
}