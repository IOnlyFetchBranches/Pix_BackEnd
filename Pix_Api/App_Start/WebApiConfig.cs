using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using pix_dtmodel.Models;

namespace Pix_Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            

            // Web API routes
            config.MapHttpAttributeRoutes();

            
            config.Routes.MapHttpRoute(
                name: "UserGetById",
                routeTemplate: "api/users/{id}",
                defaults: new { controller = "User", }
            );
            config.Routes.MapHttpRoute(
                name: "UserGetAll",
                routeTemplate: "api/users/{use}/{limit}",
                defaults: new { controller = "User" ,use = false, limit = 10}
            );

            config.Routes.MapHttpRoute(
                name: "PostPicture",
                routeTemplate: "api/pics/post",
                defaults: new {controller = "Pics"}
            );

            

        


        }
    }
}
