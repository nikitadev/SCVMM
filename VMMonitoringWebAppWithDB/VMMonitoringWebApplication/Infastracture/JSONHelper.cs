namespace VMMonitoringWebApplication.Infastracture
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public static class JSONHelper
    {
        public static string ToJSON(this object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
        }

        public static string ToJSON(this object obj, int recursionDepth)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                MaxDepth = recursionDepth,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
        }

        public static T Parse<T>(this string json)
        {
            return JObject.Parse(json).ToObject<T>();
        }

        public static object Parse(this string json, Type type)
        {
            return JObject.Parse(json).ToObject(type);
        }

        public static T FromJSON<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
        }

        public static object FromJSON(this string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                MaxDepth = 3
            });
        }
    }
}