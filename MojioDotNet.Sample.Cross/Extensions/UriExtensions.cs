using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojioDotNet.Sample.Cross.Extensions
{
    public static class UriExtensions
    {
        public static Dictionary<string, string> ParseQueryString(this System.Uri uri)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            var query = uri.Query;
            if (!string.IsNullOrEmpty(query))
            {
                if (query.StartsWith("?")) query = query.Substring(1);
                query = query.Trim();
                if (!string.IsNullOrEmpty(query))
                {
                    var parts = query.Split('&');
                    foreach (var p in parts)
                    {
                        var both = p.Split('=');
                        if (both.Length <= 1)
                        {
                            list.Add(both[0], null);
                        }
                        else
                        {
                            list.Add(both[0], both[1]);
                        }
                    }
                }
            }
            return list;
        }
    }
}
