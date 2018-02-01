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
            

            //Our "HomePage"
            config.Routes.MapHttpRoute(
                name: "Root",
                routeTemplate: "",
                defaults: new { controller = "Root"}
            );
            //PUT:TokenLogins POST: Nothing until site 
            config.Routes.MapHttpRoute(
                name: "TokenLogin",
                routeTemplate: "api/login/token",
                defaults: new { controller = "Login" }
            );



            config.Routes.MapHttpRoute(
                name: "UserGetById",
                routeTemplate: "api/users/id/{id}",
                defaults: new { controller = "User", }
            );
            config.Routes.MapHttpRoute(
                name: "UserGetAll",
                routeTemplate: "api/users/limit/{limit}",
                defaults: new { controller = "User" ,use = false, limit = 10}
            );

            
            config.Routes.MapHttpRoute(
                name: "GetUsersByPage",
                routeTemplate: "api/users/{pagenum}/{itemsperpage}",
                defaults: new {controller = "User", itemsperpage = 10, pagenum = 1}
                
            );

            config.Routes.MapHttpRoute(
                name: "GetPicsFromUserByIndex",
                routeTemplate: "api/pics/{uid}/{pagenum}/{itemsperpage}",
                defaults: new {controller = "Pics", itemsperpage = 10, pagenum = 1}
            );

            config.Routes.MapHttpRoute(
                name: "PostPicture",
                routeTemplate: "api/pics/post/create",
                defaults: new {controller = "Pics"}
            );
            


        }
    }
}
