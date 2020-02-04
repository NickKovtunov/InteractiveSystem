using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace interactiveSystem
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "Image",
                routeTemplate: "api/{controller}/{action}/{folderName}/{fileName}/",
                defaults : new { folderName = "1", fileName = "2" }
            //defaults: new { folderName = RouteParameter. }
            );

            config.Routes.MapHttpRoute(
                name: "Book",
                routeTemplate: "api/{controller}/{action}/{orgFolderName}/{folderName}/{fileName}/",
                defaults: new { orgFolderName="1",folderName = "1", fileName = "2" }
            //defaults: new { folderName = RouteParameter. }
            );
        }
    }
}
