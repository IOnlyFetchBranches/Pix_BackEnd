using System;
using System.Collections.Generic;
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

        private Session<pix_dtmodel.Models.SessionToken> tokenSession;

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

        //Creates and returns new User by Email
        
        public async Task<pix_dtmodel.Models.User> CreateNewUser(string email, string hashPass, string uname)
        {
            //Create model
            pix_dtmodel.Models.User usr = new pix_dtmodel.Models.User();


            //Set fields
            usr.Uid = ID.GenUid(email); //GenUID

            //Check backend for duplicates
            if (userSession.GetRecordById(usr.Uid).Result != null)
            {
                //If query that returned a result than the user is taken return error
                return null;
            }
            //Create secure hash.
            hashPass = Hashing.SecureHash(hashPass);

            //Always store email in lowercase
            usr.Email = email.ToLower();
            //Always store username in lowercase
            usr.Username = uname.ToLower();

            //Contact google boi
            FirebaseAuthLink tempLink = await auth.CreateUserWithEmailAndPasswordAsync(email, hashPass, uname, true);
            
            //Now we've issued a request to create a new account using the hashed password, email and then a verification email will hopefully be issued out.
            usr.TimeLeft = tempLink.ExpiresIn + ""; //Expirary Date, refresh token when this gets too low.
            //apply token.
            usr.Token = tempLink.FirebaseToken;
            //set hash
            usr.HashWord = hashPass;
            //set Google id.
            usr.Gid = tempLink.User.LocalId;
            //Finally add to dbase
            await userSession.Add(usr);


            await tokenSession.Add(new SessionToken()
            {
                Created = tempLink.Created,
                Expires = DateTime.Today.AddDays(Double.Parse(usr.TimeLeft)),
                Token = usr.Token
            });

            



            //debug line
            Console.WriteLine("User " + uname + " expires in " + usr.TimeLeft);




            return usr;
        }

        public async Task<pix_dtmodel.Models.User> SignInUser(string email, string hashpass)
        {
            //At this point hashpass should be a single hashed b64
            try
            {
                var link = await auth.SignInWithEmailAndPasswordAsync(email, hashpass);
                var timeLeft = link.ExpiresIn;
                var uid = Gen.ID.GenUid(link.User.Email.ToLower());

                //retrieve user by id
                return await userSession.GetRecordById(uid);

            }
            catch (Exception e)
            {
                return null;

            }


        }

    

        public async Task<string> getNewToken(SessionToken token , string email , string hash)
        {
            if (DateTime.Today.Month == token.Expires.Month
                    && DateTime.Today.Day == token.Expires.Hour)
                {
                    var usr = await auth.SignInWithEmailAndPasswordAsync(email, hash);
                    var newAuth = await usr.GetFreshAuthAsync();
                    return newAuth.FirebaseToken;

                }



           
        
        }

        public bool isExpired(SessionToken token)
        {
            if (DateTime.Today.Month == token.Expires.Month
                && DateTime.Today.Day == token.Expires.Hour)
            {
                return true;
            }

            return false;

        }
        //Refresh a session
        public async Task<bool> VerifySession(SessionToken session)
        {
            return await tokenSession.CheckFieldFrom(session.Token, Defaults.Fields.Users.Uid, session.Uid);
        }

        //Verify Token
        public async Task<bool> VerifyToken(string token, string uid)
        {
            bool valid = await tokenSession.CheckFieldFrom(token, Defaults.Fields.Users.Uid, uid);
            var cred = await tokenSession.GetRecordById(token);

            if (isExpired(cred))
            {
                valid = false;
            }
            
            

            return valid;
        }

        //Verifies Pics
        public async Task<bool> VerifyPost(Pic post)
        {

            return await tokenSession.CheckFieldFrom(post.Token, Defaults.Fields.Users.Uid, post.Uid);



        }


        
    }
}
