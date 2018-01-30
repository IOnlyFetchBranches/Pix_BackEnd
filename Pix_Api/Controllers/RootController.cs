using System;
using pix_dtmodel.Models;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Razor.Parser;
using System.Web.Razor.Text;
using RazorEngine;

namespace Pix_Api.Controllers
{
    public class RootController : ApiController
    {

       
        [System.Web.Http.HttpGet]
        public HttpResponseMessage welcome()
        {
            try
            {
                HttpResponseMessage res = new HttpResponseMessage(HttpStatusCode.OK);

                dynamic model = new {date = DateTime.Now};
                //Grab view from html dir, using HttpContext to format it
                var ViewPath = HttpContext.Current.Server.MapPath("~\\Static\\Pages\\apiroot.cshtml");
                
                //Load File
                var template = File.ReadAllText(ViewPath);

                //use Razor to Parse it into proper html
                var html = Razor.Parse(template, model);


                res.Content = new StringContent(html);

                res.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");




                //Send back to client
                return res;
            }
            catch (Exception e)
            {
                
                //If anything funky happens return safe error;
                var errRes = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                //in the future we can do some gymnastics to manually write a specific line in a more pretty  
                errRes.Content = new StringContent("<!DOCTYPE html> <html> <title> Whoops! </title> <body> "+ e.InnerException.Message + "</body> </html>");
                //Set headers
                errRes.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                //Send error page.
                return errRes;
            }

        }

    
    }
}
