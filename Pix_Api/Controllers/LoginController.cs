using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using pix_sec;

namespace Pix_Api.Controllers
{
    public class LoginController : ApiController
    {

    
        //New Login Method
        [HttpPut]
        public async Task<HttpResponseMessage> TokenLogin()
        {
            try
            {
                HttpResponseMessage okRes = new HttpResponseMessage(HttpStatusCode.Accepted);

                //Load token 
                var rawContent = await Request.Content.ReadAsStringAsync();

                //Login
                var user = LoginManager.getInstance().LoginWithToken(rawContent);

                return okRes;
            }catch(Exception e)
            {
                Debug.WriteLine(e.Message,"TokenLogin");

                HttpResponseMessage errRess = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errRess.Content = new StringContent(e.Message);
                return errRess;
            }
        }

        














        /*
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
    

    */




    }



}
