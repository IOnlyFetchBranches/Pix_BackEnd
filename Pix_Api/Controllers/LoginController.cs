using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using pix_dtmodel.Connectors;
using pix_dtmodel.Managers;
using pix_dtmodel.Models;
using pix_sec;
using pix_sec.Gen;

namespace Pix_Api.Controllers
{
    public class LoginController : ApiController

    {

        //Open a new session
        private static Session<User> userSession = new Session<User>(
            DataManager.Init(Defaults.ConnectionStrings.Localhost, Defaults.DatabaseNames.TestProd, null),
            Defaults.Collections.Users
        );

        private static LoginManager manager = LoginManager.getInstance();



        [HttpPost]
        [ActionName("CreateUser")]
        public  async Task<HttpResponseMessage> CreateUser()
        {
            var res = new HttpResponseMessage(HttpStatusCode.Accepted);

            var headers = Request.Content.Headers.ContentType;

            if (headers == MediaTypeHeaderValue.Parse("application/json"))
            {

                //Load Json data (Possible security checkpoint)
                var raw = Request.Content.ReadAsStringAsync().Result;
                //read the json
                Dictionary<string, string> json = JsonConvert.DeserializeObject<Dictionary<string, string>>(raw);
                //Grab Login Manager
                LoginManager manager = LoginManager.getInstance();
                //Parse fields
                string email = json["email"];
                string hash = json["hash"];
                string uname = json["uname"];

                //create user
                try
                {
                    var newUser = await manager.CreateNewUser(email, hash, uname);

                    //Respond with session information

                    //Set header
                    res.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    //Instance json model class
                    Dictionary<string, string> resJson = new Dictionary<string, string>();

                    //Populate json model
                    resJson.Add("token", newUser.Token);
                    resJson.Add("uid", newUser.Uid);
                    resJson.Add("expiresIn",newUser.TimeLeft);
                    resJson.Add("status",newUser.IsBanned+"");

                    //Attach
                    res.Content = new StringContent(JsonConvert.SerializeObject(resJson));
                    //Send 
                    return res;
                }
                catch (Exception e)
                {
                    HttpResponseMessage errRes = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    errRes.Content = new StringContent("Error Processing Values \n" + e.Message);
                    return errRes;
                }

               
            }
            else
            {
                var errRes = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errRes.Content = new StringContent("Invalid payload!");
                return errRes;
            }


           

        }

        [HttpPut]
        [ActionName("LoginUser")]
        public async Task<HttpResponseMessage> LoginUser()
        {
            try
            {
                if (Request.Content.Headers.ContentType == MediaTypeHeaderValue.Parse("application/json"))
                {
                    string content = await Request.Content.ReadAsStringAsync();

                    Dictionary<string, string>
                        json = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

                    //Load fields here
                    string email = json["email"];
                    string hash = json["hash"];

                    //Hash will be b64 converted 1X hash of pass
                    
                    //Check this against the database, after rehashing
                    string actualHash = Hashing.SecureHash(hash);

                    //Login
                    var sessionInfo = await manager.SignInUser(email, hash);
                

                    

                    if (sessionInfo == null)
                    {
                        throw new SecurityException("Invalid or Illegal Credentials!");
                    }
                    else
                    {
            
                        //Create Response Json Model
                        var resRaw = new Dictionary<string, string>();
                        resRaw.Add("uid",sessionInfo.Uid);
                        resRaw.Add("token", sessionInfo.Token);
                        resRaw.Add("expiresIn", sessionInfo.TimeLeft);
                        resRaw.Add("status",sessionInfo.Status);

                        //Serialize json

                        string resJson = JsonConvert.SerializeObject(resRaw);

                        //Generate Response (Session info)
                        HttpResponseMessage okRes = new HttpResponseMessage(HttpStatusCode.OK);
                        okRes.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        okRes.Content = new StringContent(resJson);


                        return okRes;


                    }



                }
                else
                {
                    var errRes = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    errRes.Content = new StringContent("Request is Malformatted/Not Valid Json!");
                    return errRes;
                }
            }
            catch (Exception e)
            {
                var errRes = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errRes.Content = new StringContent("Request is Malformatted/Not Valid Json! \n"+e.Message);
                
                return errRes;
            }

        }

        [HttpPut]
        [ActionName("LoginToken")]
        public async Task<HttpResponseMessage> LoginToken(string token)
        {
            try
            {
                if (Request.Content.Headers.ContentType == MediaTypeHeaderValue.Parse("application/json"))
                {

                    //if json
                    var rawContent = await Request.Content.ReadAsStringAsync();
                    //Deserialize Content
                    Dictionary<string, string> reqJson =
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(rawContent);

                    //Should contain uid field

                    var uid = reqJson["uid"];
                   
                    //Test Existence 
                    pix_sec.Rules.Assert.AssertExists(uid, token);

                    //Attempt login
                    bool isGood = await manager.VerifyToken(token, uid);

                    if (!isGood)
                    {
                        throw new SecurityException("Token is not valid... \n"+token);
                    }
                
                    //Authorize the app to continue with the current credentials.
                    return new HttpResponseMessage(HttpStatusCode.OK);



                }
                else
                {
                    throw new JsonException("Expected Json Header...");
                }
            }
            catch (Exception e)
            {
                var errRes = new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                errRes.Content = new StringContent(e.Message);
                errRes.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                return errRes;

            }

        }


    }
}
