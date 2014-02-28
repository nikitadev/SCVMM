namespace VMMonitoringWebApplication.Infastracture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using VMMonitoringWebApplication.Models;

    public static class DefaultNames
    {
        public const string ServerCookieName = "VMMServerName";
        public const string VMName = "VMName";
    }

    public class CookieRememberService : IRememberService
    {
        public void Add<T>(string cookieName, T obj)
        {
            var jsonModel = obj.ToJSON();
            
            var newCookie = new HttpCookie(cookieName, jsonModel);

            var cookie = HttpContext.Current.Request.Cookies.Get(cookieName);

            if (cookie == null)
            {
                HttpContext.Current.Request.Cookies.Add(newCookie);
                HttpContext.Current.Response.Cookies.Add(newCookie);
            }
            else
            {
                HttpContext.Current.Request.Cookies.Set(newCookie);
            }

            
        }

        /// <summary>
        /// Gets the user from cookie.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">HttpContext.Current</exception>
        public IEnumerable<T> GetListFromCookies<T>(string cookieName)
        {
            if (HttpContext.Current == null)
                throw new ArgumentNullException("HttpContext.Current");

            if (String.IsNullOrEmpty(cookieName))
                throw new ArgumentNullException("CookieName");

            var cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                yield return cookie.Value.FromJSON<T>();
            }
        }

        public bool Check(string cookieName)
        {
            return HttpContext.Current.Request.Cookies.AllKeys.Any(k => k.Equals(cookieName));
        }

        public T Get<T>(string cookieName)
        {
            var cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                return cookie.Value.FromJSON<T>();
            }

            return default(T);
        }
    }
}