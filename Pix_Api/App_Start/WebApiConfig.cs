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
                name: "PostToken",
                routeTemplate: "login/token",
                defaults: new { controller = "Login" }
            );

            //Not in use currently
            config.Routes.MapHttpRoute(
                name: "Get",
                routeTemplate: "login/",
                defaults: new { controller = "Login" }
            );



            config.Routes.MapHttpRoute(
                name: "UserGetById",
                routeTemplate: "users/id/{id}",
                defaults: new { controller = "User", }
            );
            config.Routes.MapHttpRoute(
                name: "UserGetAll",
                routeTemplate: "users/limit/{limit}",
                defaults: new { controller = "User" , limit = 10}
            );

            
            config.Routes.MapHttpRoute(
                name: "GetUsersByPage",
                routeTemplate: "users/page/{pagenum}/{itemsperpage}",
                defaults: new {controller = "User", itemsperpage = 10, pagenum = 1}
                
            );

            config.Routes.MapHttpRoute(
                name: "GetPicsFromUserByPage",
                routeTemplate: "pics/{uid}/{pagenum}/{itemsperpage}",
                defaults: new {controller = "Pics", itemsperpage = 10, pagenum = 1}
            );

            config.Routes.MapHttpRoute(
                name: "GetPicsFromUsersByPage",
                routeTemplate: "pics/{pagenum}/{itemsperpage}",
                defaults: new { controller = "Pics", itemsperpage = 10, pagenum = 1 }
            );

            config.Routes.MapHttpRoute(
                name: "PostPicture",
                routeTemplate: "pics/legacy/post/",
                defaults: new {controller = "Pics"}
            );

             config.Routes.MapHttpRoute(
                name: "PostMetadata",
                routeTemplate: "pics/upload/begin",
                defaults: new {controller = "Pics", action = "begin"}
            );
            config.Routes.MapHttpRoute(
                name: "BeginUpload",
                routeTemplate: "pics/upload/go/{picId}",
                defaults: new {controller = "Pics", picId = ""}
            );
            config.Routes.MapHttpRoute(
                name: "GetPicsByRegion",
                routeTemplate: "pics/by/region", //This will also require an additional &&{token} but not during debugging
                defaults: new {controller = "Pics", action= "GetPicsByRegion" }
            );

            //Return image as a Byte stream
            config.Routes.MapHttpRoute(
                name: "StreamImageById",
                routeTemplate: "pics/id={id}",
                defaults: new {controller = "Pics", id=""}
            );
            //Return image in b64
            config.Routes.MapHttpRoute(
                name: "GetImageById",
                routeTemplate: "pics/{id}",
                defaults: new { controller = "Pics" }
            );
            




        }
    }
}
