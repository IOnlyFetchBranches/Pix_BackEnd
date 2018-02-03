using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using FirebaseNetAdmin;
using FirebaseNetAdmin.Configurations.ServiceAccounts;
using JWT;
using JWT.Serializers;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using pix_dtmodel.Connectors;
using pix_dtmodel.Managers;
using pix_dtmodel.Models;
using pix_sec;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace Pix_Api.Controllers
{
    public class LoginController : ApiController
    {

        private static IMongoDatabase database =
            DataManager.Init(Defaults.ConnectionStrings.Localhost, Defaults.DatabaseNames.TestProd, null);

        private static Session<User> userSession = new Session<User>(database, Defaults.Collections.Users);
        private static Session<SessionToken> credSession = new Session<SessionToken>(database, Defaults.Collections.SessionData);

        //New Login Method
    
        [HttpPost]
        public async Task<HttpResponseMessage> PostToken()
        {

           
            try
            {




                //Load token 
                var rawContent = await Request.Content.ReadAsStringAsync();

                //Log
                Directory.CreateDirectory("C:\\Logs\\");
                var logOut = File.CreateText("C:\\Logs\\log.txt");
                logOut.Write(rawContent);
                
                var set = new Firebase.Auth.FirebaseConfig("AIzaSyBAbtFrjxR-dDNIVnSa1ilJEsE3XQuVVEQ");

                var auth = new Firebase.Auth.FirebaseAuthProvider(set);

                var tokenManager = new pix_dtmodel.Managers.Firebase.FirebaseJWTAuth(
                    "pix-55e76");

                var resContent = await tokenManager.Verify(rawContent);




               
                HttpResponseMessage okRes = new HttpResponseMessage(HttpStatusCode.Accepted);
                //okRes.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                

                //Get the auth package
                var authPack = JsonConvert.DeserializeObject<Dictionary<string,string>>
                    (pix_sec.Gen.Packager.CreateAuthPackage(resContent));

                //Check the backend
                if ( await credSession.GetRecordById(authPack["gid"]) == null)
                {
                    //if it doesnt exist [First time login] Create the records

                    User newUser = new User()
                    {
                        Additional = null,
                        Email = authPack["email"],
                        First = "",
                        Gid = authPack["gid"],
                        Last = "",
                        Uid = pix_sec.Gen.ID.GenUid(authPack["email"]),
                        Username = null,
                        Verified = bool.Parse(authPack["email_verified"])
                    };

                    //Add to backend
                    userSession.Add(newUser);

                    //Create session token for quick lookup + Add to backend.

                    await credSession.Add(new SessionToken()
                    {
                        Gid = newUser.Gid,
                        Uid = newUser.Uid
                    });

                    //Done.


                }



                //pix_sec.Gen.Packager.CreateAuthPackage(resContent)
                okRes.Content = new StringContent(pix_sec.Gen.Packager.CreateAuthPackage(resContent));


                //Respond if all went ok
                return okRes;
            }catch(Exception e)
            {
                Debug.WriteLine(e.Message,"TokenLogin");

                HttpResponseMessage errRess = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errRess.Content = new StringContent(e.Message);
                return errRess;
            }
        }


        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            var res = new HttpResponseMessage(HttpStatusCode.Accepted);
            res.Content = new StringContent(await Request.Content.ReadAsStringAsync());
            return res;
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
