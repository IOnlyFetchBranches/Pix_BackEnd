using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Newtonsoft.Json;

namespace Pix_Api.Controllers
{
    public class LoginController : ApiController
    {
        [HttpPost]
        public  HttpResponseMessage CreateUser()
        {
            var res = new HttpResponseMessage(HttpStatusCode.Accepted);

            var headers = Request.Content.Headers.ContentType;

            if (headers == MediaTypeHeaderValue.Parse("application/json"))
            {

                //Load Json data (Possible security checkpoint)
                var raw = Request.Content.ReadAsStringAsync().Result;

                var json =JsonConvert.DeserializeObject(raw);

                Debug.WriteLine(json.GetType());
                
                

                return res;
            }
            else
            {
                var errRes = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errRes.Content = new StringContent("Invalid payload!");
                return errRes;
            }


           

        }
    }
}
