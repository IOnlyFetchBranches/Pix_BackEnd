using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace pix_sec.Gen
{
      public abstract class ID
    {

        //This class should gen all ids.
        
            //Use this to return a proper UID given an email
            public static string GenUid(string email)
            {
                //Make a hasher
                SHA256 hasher = new SHA256Managed();

                hasher.Initialize(); //Init the csp

                //hash email for use in the UID
                byte[] hashedBytes = hasher.ComputeHash(Encoding.ASCII.GetBytes(email));
                //Use bit Converter to get hash string
                string hash = Convert.ToBase64String(hashedBytes);

                string hashString = hash.Substring(0, hash.Length / 2);

                //Teardown.
                hasher.Clear();

                return "USR" + hashString;
            }

        public static string GenPicId(string uid)
        {
            //Generates Pid 
            var encUid = Convert.ToBase64String(Encoding.ASCII.GetBytes(uid));
            var encSalt = Convert.ToBase64String(Encoding.ASCII.GetBytes(DateTime.Now.ToString("hh:mm:ss")
                + DateTime.Now.Millisecond + uid.Substring(0,5)));
            
            //"p<b64(uid) 0 - halfLength><b64(datetime<uid.substring 0-5>)
            return (string) "p" + encUid.Substring(0, encUid.Length / 2) + encSalt;
            


        }
        }
    
}
