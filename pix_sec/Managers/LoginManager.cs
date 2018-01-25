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

            //Make a hasher
            SHA256 hasher = new SHA256Managed();

            hasher.Initialize(); //Init the csp

            //hash email for use in the UID
            byte[] hashedBytes = hasher.ComputeHash(Encoding.ASCII.GetBytes(email));
            //Use bit Converter to get hash string
            string hashString = BitConverter.ToString(hashedBytes).Replace("-", String.Empty);

            //Set fields
            usr.Uid = "USR" + hashString;

            //Check backend for duplicates
            if (userSession.GetRecordById(usr.Uid).Result != null)
            {
                //If query that returned a result than the user is taken return error
                return null;
            }


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
            usr.HashWord = hashPass.Replace("-", String.Empty);
            //set Google id.
            usr.Gid = tempLink.User.LocalId;
            //Finally add to dbase
            userSession.Add(usr);



            //debug line
            Console.WriteLine("User " + uname + " expires in " + usr.TimeLeft);




            //cleanup?
            hasher.Clear();
            hashedBytes = null;
            hashString = null;


            return usr;
        }



        abstract class Hash
        {

            public static string SecureHash(byte[] rawWord)
            {
                var hasher = new SHA256Managed();
                //hash email for use in the UID
                byte[] fHashedBytes = hasher.ComputeHash(rawWord);

                string firstHashed = BitConverter.ToString(fHashedBytes).Replace("-", String.Empty);

                byte[] hashedBytes = hasher.ComputeHash(Encoding.ASCII.GetBytes(firstHashed));

                //Use bit Converter to get hash string
                string hashString = BitConverter.ToString(hashedBytes).Replace("-", String.Empty);

                return hashString;

            }








        }

        abstract class ID
        {
            public static string GenUid(string email)
            {
                //Make a hasher
                SHA256 hasher = new SHA256Managed();

                hasher.Initialize(); //Init the csp

                //hash email for use in the UID
                byte[] hashedBytes = hasher.ComputeHash(Encoding.ASCII.GetBytes(email));
                //Use bit Converter to get hash string
                string hashString = BitConverter.ToString(hashedBytes).Replace("-", String.Empty);


                return "USR" + hashString;
            }
        }
    }
}
