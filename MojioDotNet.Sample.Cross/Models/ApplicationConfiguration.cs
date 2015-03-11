
using System;

namespace MojioDotNet.Sample.Cross.Models
{
    public class ApplicationConfiguration
    {
        public ApplicationConfiguration()
        {
            ApplicationId = "9fef025f-966b-4ab7-b048-09cbca6a5faf";
            Uri uri = null;
            System.Uri.TryCreate("http://localhost/_done?", UriKind.RelativeOrAbsolute, out uri);
            RedirectUri = uri;
            Live = false;
            BingMapCredentials = "AiVIP0IcaljvepBb6VRlOFNBLtC8HEbqavrDWAxc5nI2Am2XprFY2rv5PfZb5buw";
        }
        public string ApplicationId { get; set; }
        public System.Uri RedirectUri { get; set; }
        public bool Live { get; set; }
        public string BingMapCredentials { get; set; }
        public System.Uri AuthorizeUri { get; set; }
    }
}
