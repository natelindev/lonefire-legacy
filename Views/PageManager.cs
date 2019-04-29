using System;
using System.Collections.Generic;
using lonefire.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace lonefire.Views
{
    public static class PageManager
    {

        public static bool UseSideBar(ViewContext viewContext)
        {
            var controllerName = viewContext.RouteData.Values["controller"] as string;
            var actionName = viewContext.RouteData.Values["action"] as string;
            return controllerName == "Article" && actionName == "Index" ||
                    controllerName == "Article" && actionName == "Billboard" ||
                   controllerName == "User" 
            ;
        }

        public static string IsActive(ViewContext viewContext,string controller)
        {
            var controllerName = viewContext.RouteData.Values["controller"] as string;
            return string.Equals(controllerName, controller, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }

        public static string IsActive(ViewContext viewContext, string controller, string action)
        {
            var controllerName = viewContext.RouteData.Values["controller"] as string;
            var actionName = viewContext.RouteData.Values["action"] as string;

            return 
            (string.Equals(controllerName, controller, StringComparison.OrdinalIgnoreCase) &&
             string.Equals(actionName, action, StringComparison.OrdinalIgnoreCase))
             ? "active" : null;
        }

        public static string IsActive(ViewContext viewContext, string controller,params string[] actions)
        {
            var controllerName = viewContext.RouteData.Values["controller"] as string;
            var actionName = viewContext.RouteData.Values["action"] as string;

            bool is_active = false;

            if (string.Equals(controllerName, controller, StringComparison.OrdinalIgnoreCase))
            {
                foreach (string a in actions)
                {
                    if (string.Equals(actionName, a, StringComparison.OrdinalIgnoreCase))
                    {
                        is_active = true;
                        break;
                    }
                }
            }

            return is_active ? "active" : null;
        }

        public static string GetReturnUrl(ViewContext v)
        {
            string url = "/";
            url += v.RouteData.Values["Controller"] as string;
            url += "/";
            url += v.RouteData.Values["action"] as string;
            url += "/";
            url += v.RouteData.Values["id"] as string;
            
            return v.RouteData.Values["Controller"] as string == "Account" ? null : url;
        }

    }
}
