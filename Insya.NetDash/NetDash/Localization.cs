// Copyright (c) 2014, Insya Interaktif.
// Developer @yasinkuyu
// All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Insya.NetDash
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Localization html helper
        /// </summary>
        /// <param name="helper">object</param>
        /// <param name="name">string</param>
        /// <returns></returns>
        public static MvcHtmlString Localize(this HtmlHelper helper, string name)
        {
            return new MvcHtmlString(Localization.Get(name));
        }

        /// <summary>
        /// Setting file html helper
        /// </summary>
        /// <param name="helper">object</param>
        /// <param name="name">string</param>
        /// <returns></returns>
         public static MvcHtmlString Setting(this HtmlHelper helper, string name)
        {
            return new MvcHtmlString(Settings.Get(name));
        }
        
    }

    public static class Localization
    {
        /// <summary>
        /// Localization txt get item
        /// </summary>
        /// <param name="item">string</param>
        /// <returns></returns>
        public static string Get(string item)
        {
            try
            {

                var dictionary = new Dictionary<string, string>();
                var path = HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data/Localization/" + Settings.Get("LANGUAGE") + ".txt");
                var regex = new Regex(@"((?<Name>[\w|\.]*)\s*=\s*(?<Value>.*?))$");

                //HttpContext.Current.Response.Write(path);

                using (var reader = new StreamReader(path))
                {

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var match = regex.Match(line);
                        if (match.Success)
                        {
                            dictionary.Add(match.Groups[2].Value, match.Groups[3].Value);
                        }
                    }

                }

                foreach (var pair in dictionary.Where(x => x.Key == item))
                {
                    return pair.Value;
                }

            }
            catch
            {
                // TODO: exception handle.
                return item; 
            }
            return item;
        }
        
    }
}
