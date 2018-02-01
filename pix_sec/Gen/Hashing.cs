using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace pix_sec.Gen
{
    /*
     * Generator for all hashing to be done.
     * hash email for use in the UID *******
     */
    public abstract class Hashing
    {

            public static string SecureHash(byte[] rawWord)
            {
                var hasher = new SHA256Managed();
               
                byte[] fHashedBytes = hasher.ComputeHash(rawWord);

                string firstHashed = Convert.ToBase64String(fHashedBytes);

                byte[] hashedBytes = hasher.ComputeHash(Encoding.ASCII.GetBytes(firstHashed));

                //Use bit Converter to get hash string
                string hashString = Convert.ToBase64String(hashedBytes);

                return hashString;

            }

        public static string SecureHash(string b64RawWord)
        {
            var hasher = new SHA256Managed();
           
            byte[] fHashedBytes = hasher.ComputeHash(Encoding.ASCII.GetBytes(b64RawWord));

            string firstHashed = Convert.ToBase64String(fHashedBytes);

            byte[] hashedBytes = hasher.ComputeHash(Encoding.ASCII.GetBytes(firstHashed));

            //Use bit Converter to get hash string
            string hashString = Convert.ToBase64String(hashedBytes);

            return hashString;

        }









    }
}
