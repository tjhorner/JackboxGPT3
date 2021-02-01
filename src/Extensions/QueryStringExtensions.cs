using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace JackboxGPT3.Extensions
{
    public static class QueryStringExtensions
    {
        public static string AsQueryString(this object serializable)
        {
            var serialized = JsonConvert.SerializeObject(serializable);
            var deserialized = JsonConvert.DeserializeObject<IDictionary<string, string>>(serialized);
            var encoded = deserialized.Select(x => HttpUtility.UrlEncode(x.Key) + "=" + HttpUtility.UrlEncode(x.Value));

            return string.Join("&", encoded);
        }
    }
}
