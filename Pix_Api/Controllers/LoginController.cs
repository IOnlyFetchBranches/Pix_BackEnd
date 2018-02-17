using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.AccessControl;
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
using pix_sec.Gen;
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
        //Object reference not set == Verify Failure.... Incorect Token!
    
        [HttpPost]
        public async Task<HttpResponseMessage> PostToken()
        {
            //Open LogFile
            var LogFile = new FileStream("C:\\DebugLogs\\" + "log" + DateTime.Now.ToString("MM-dd-yyyy") + "-" 
                + DateTime.Now.ToString("h-mm-ss") +  " " +DateTime.Now.Millisecond + ".txt", 
                        FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.Read);

            var bytes = await Request.Content.ReadAsByteArrayAsync();


            try
            {

            
                
                //Get time
                var beginTime = DateTime.Now;

                


                //Load token 
                var rawContent = Encoding.UTF8.GetString(bytes);

                //Sanitize (if neccesary)
                if (rawContent.Contains("data="))
                    rawContent = rawContent.Replace("data=", "");

                //Log
                Logger.Log(rawContent, LogFile);

                //log LOGLOG log!!L!O!G! [It helps my sanity <3]
                Logger.Log("Loaded Data!",LogFile);

                Logger.Log("Grabbing Key...", LogFile);
                var set = new Firebase.Auth.FirebaseConfig( await Defaults.System.GetApiKey());
                Logger.Log("Loaded Key!", LogFile);


                var auth = new Firebase.Auth.FirebaseAuthProvider(set);

                Logger.Log("Init Firebase Auth...", LogFile);

                var tokenManager = new pix_dtmodel.Managers.Firebase.FirebaseJWTAuth(
                    "pix-55e76");

                Logger.Log("Verifying Token...",LogFile);
                var resContent = await tokenManager.Verify(rawContent);

                if (resContent != null)
                    Logger.Log("Token Verified!", LogFile);
                else
                {
                    throw new Exception("Was unable to parse the response token!");
                }




                HttpResponseMessage okRes = new HttpResponseMessage(HttpStatusCode.Accepted);
                //okRes.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                Logger.Log("Building Message Pack with: \n" + pix_sec.Gen.Packager.CreateAuthPackage(resContent,LogFile), LogFile);


                //Get the auth package
                var authPack = resContent;

            
                



                Logger.Log("Done!", LogFile);


                bool isNewUser = false;

                //Find User
                var record = await credSession.GetRecordById(authPack["gid"]);

                //Check the backend
                if ( record == null)
                {
                    Logger.Log("Generating New User...", LogFile);
                    //if it doesnt exist [First time login] Create the records

                    User newUser = new User()
                    {
                        Additional = null,
                        Email = authPack["email"],
                        First = "",
                        Gid = authPack["gid"],
                        Last = "",
                        Uid = pix_sec.Gen.ID.GenUid(authPack["email"]),
                        Username = "", // Ask for it later...
                        Verified = bool.Parse(authPack["email_verified"])

                    };

                    authPack.Add("uid",newUser.Uid);

                    isNewUser = true; //This will ('Should') signal client to request and then post a New Username.



                   


                    Logger.Log("Adding to backend...", LogFile);
                    //Add to backend
                    userSession.Add(newUser);

                    Logger.Log("User Added!", LogFile);

                    //Create session token for quick lookup + Add to backend.

                    await credSession.Add(new SessionToken()
                    {
                        Gid = newUser.Gid,
                        Uid = newUser.Uid
                    });

                    //Done.

                }


                Logger.Log("Building Response...", LogFile);

                if (!isNewUser)
                    authPack.Add("uid", record.Uid);

                //Get final time
                var timeTaken = (DateTime.Now - beginTime).Milliseconds;

                //Add server messages here
                authPack.Add("isNewUser", isNewUser + "");
                authPack.Add("requestTime", timeTaken + "ms");


                //pix_sec.Gen.Packager.CreateAuthPackage(resContent)
                try
                {
                    okRes.Content = new StringContent(JsonConvert.SerializeObject(authPack));
                    Logger.Log("Sent!", LogFile);

                    //Done
                    LogFile.Close();
                    //Respond all went ok
                    return okRes;

                    
                }
                catch (Exception e)
                {
                    HttpResponseMessage errRess = new HttpResponseMessage(HttpStatusCode.Continue);
                    errRess.Content = new StringContent(e.Message);
                    Logger.Log("An Error Occured, But user is verified..." + e.Message, LogFile);
                    LogFile.Close();
                    return errRess;
                }

                
                
              
            }catch(Exception e)
            {


                //close (Could be condensed into a using!)
                
                Debug.WriteLine(e.Message,"TokenLogin");

                HttpResponseMessage errRess = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errRess.Content = new StringContent(e.Message + " " +e.Source + " " + e.StackTrace);
                Logger.Log("An Error Occured :( /n" +e.Message, LogFile);
                LogFile.Close();
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
