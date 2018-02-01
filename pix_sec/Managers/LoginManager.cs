using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using pix_dtmodel.Models;
using pix_dtmodel.Managers;
using pix_dtmodel.Connectors;
using Firebase.Auth;
using System.Net.Http;
using pix_sec.Gen;

namespace pix_sec
{
    //DONTUSESHA-1 FORHASHES >:(
    public sealed class LoginManager
    {

        //Declare our new session, expecting and updating Users + Tokens
        private Session<pix_dtmodel.Models.User> userSession;

        

        //Database connection
        private IMongoDatabase database;

        private FirebaseAuthProvider auth;


        //Singleton Instance
        private static LoginManager instance;

        //Singleton Getter
        public static LoginManager getInstance()
        {
            if (instance == null)
            {
                return new LoginManager();
            }
            else
            {
                return instance;
            }
        }

        //Constructor
        private LoginManager()
        {
            //Private constructor
            database = DataManager.Init(Defaults.ConnectionStrings.Localhost, Defaults.DatabaseNames.TestProd, null);
            //Define a new session with "User Models" on the "pix_users" collection
            userSession = new Session<pix_dtmodel.Models.User>(database, "pix_users");
            //Define a new session with "Session Token Models" on the "pix_tokens" collection
            tokenSession = new Session<SessionToken>(database, "pix_tokens");

            //Begin Google Session
            string k = Defaults.System.ApiKey; //get api key from backend
            try
            {
                auth = new FirebaseAuthProvider(new FirebaseConfig(k));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error has occured [AuthProc =>" + e.Message);
            }



        }



        public async Task<pix_dtmodel.Models.User> LoginWithToken(string encodedToken)
        {
            try
            {
                var link = await auth.SignInWithCustomTokenAsync(encodedToken);

               //If its valid, add to server if it doesnt exist or pull existing record

                var user = await userSession.GetRecordById(Gen.ID.GenUid(link.User.Email));

                if (user == null)
                {
                    //It doesnt exist so add it
                    var newUser = new pix_dtmodel.Models.User();


                    newUser.Email = link.User.Email;
                    newUser.Uid = ID.GenUid(link.User.Email);
                    newUser.Additional = null;
                    newUser.First = link.User.FirstName;
                    newUser.Last = link.User.LastName;
                    newUser.Gid = link.User.FederatedId;
                    newUser.Username = link.User.DisplayName;
                    newUser.Verified = link.User.IsEmailVerified;

                    userSession.Add(newUser);

                    return newUser;

                }
                else
                {
                    return user;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error with token login! \n" +e);
                throw e;
            }

        }





        //Verifies Pics
        public async Task<bool> VerifyPost(Pic post)
        {

            return  userSession.GetRecordById(post.Uid).Result != null;



        }
    }
}
