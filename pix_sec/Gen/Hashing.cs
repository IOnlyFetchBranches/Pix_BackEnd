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
     */
    public abstract class Hashing
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
}
