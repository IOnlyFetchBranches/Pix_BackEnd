using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pix_dtmodel.Managers.Firebase
{
    //thanks to -> https://gist.github.com/saltyJeff/41029c9facf3ba6159ac019c1a85711a#file-firebasejwtauth-cs-L48
    //Had to make several modifications but this provided a valuable starting point...
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Net.Http;
    using Newtonsoft.Json;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Cryptography;

    public class FirebaseJWTAuth
    {
        public string FirebaseId;
        private HttpClient Req;

        public FirebaseJWTAuth(string firebaseId)
        {
            FirebaseId = firebaseId;
            Req = new HttpClient();
            Req.BaseAddress = new Uri("https://www.googleapis.com/robot/v1/metadata/");
        }

        public async Task<Dictionary<string,string>> Verify(string token)
        {
            //following instructions from https://firebase.google.com/docs/auth/admin/verify-id-tokens
            string hashChunk = token; //keep for hashing later on
            hashChunk = hashChunk.Substring(0, hashChunk.LastIndexOf('.'));
            token = token.Replace('-', '+').Replace('_', '/'); //sanitize for base64 on C#
            string[] sections = token.Split('.'); //split into 3 sections according to JWT standards
            JwtHeader header = B64Json<JwtHeader>(sections[0]);

            if (header.alg != "RS256")
            {
                return null;
            }

            HttpResponseMessage res = await Req.GetAsync("x509/securetoken@system.gserviceaccount.com");
            string keyDictStr = await res.Content.ReadAsStringAsync();
            Dictionary<string, string> keyDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(keyDictStr);
            string keyStr = null;
            keyDict.TryGetValue(header.kid, out keyStr);
            if (keyStr == null)
            {
                return null;
            }

            using (var rsaCrypto = CertFromPem(keyStr))
            {
                using (var hasher = SHA256Managed.Create())
                {
                    byte[] plainText = Encoding.UTF8.GetBytes(hashChunk);
                    byte[] hashed = hasher.ComputeHash(plainText);

                    byte[] challenge = SafeB64Decode(sections[2]);

                    RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsaCrypto);
                    rsaDeformatter.SetHashAlgorithm("SHA256");
                    if (!rsaDeformatter.VerifySignature(hashed, challenge))
                    {
                        return null;
                    }
                }
            }

            JwtPayload payload = B64Json<JwtPayload>(sections[1]);
            long currentTime = EpochSec();
            //the if chain of death
            if (payload.aud != FirebaseId ||
             payload.iss != "https://securetoken.google.com/" + FirebaseId ||
             payload.iat >= currentTime ||
             payload.exp <= currentTime)
            {
                return null;
            }

          
            //Grab payload details and return.

            Dictionary<string, string> dict = new Dictionary<string, string>();


            dict.Add("gid", payload.sub);
            dict.Add("email", payload.email);
            dict.Add("email_verified",payload.email_verified);


            return dict;


        }

        private static RSACryptoServiceProvider CertFromPem(string pemKey)
        {
            X509Certificate2 cert = new X509Certificate2();
            cert.Import(Encoding.UTF8.GetBytes(pemKey));
            return (RSACryptoServiceProvider)cert.PublicKey.Key;
        }

        public static byte[] SafeB64Decode(string encoded)
        {
            //Console.WriteLine(encoded);
            //Sanitize
            if (encoded.EndsWith("\r\n"))
            {
                var newEncoded =
                    encoded.Replace("\r\n",
                        ""); // encoded.Replace("\r\n", "") + new string('=', 4 - encoded.Length % 4)
                if (newEncoded.Length % 4 != 0)
                {
                    string newEncodedPad =
                        newEncoded + new string('=', 4 - newEncoded.Length % 4); //'=', 4 - encoded.Length % 4
                    return Convert.FromBase64String(newEncodedPad);
                }

                return Convert.FromBase64String(newEncoded);
            }



            if (encoded.Length % 4 != 0)
                {
                    string encodedPad =
                        encoded + new string('=', 4 - encoded.Length % 4); //'=', 4 - encoded.Length % 4
                    return Convert.FromBase64String(encodedPad);
                }

                // Console.WriteLine(encodedPad);
            return Convert.FromBase64String(encoded);

        }
            

    
           

            
        
        private static string SafeB64DecodeStr(string encoded)
        {
            return Encoding.UTF8.GetString(SafeB64Decode(encoded));
        }
        private static T B64Json<T>(string encoded)
        {
            string decoded = SafeB64DecodeStr(encoded);
            return JsonConvert.DeserializeObject<T>(decoded);
        }
        private static DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long EpochSec()
        {
            return (long)((DateTime.UtcNow - Jan1st1970).TotalSeconds);
        }
        private struct JwtHeader
        {
            public string alg;
            public string kid;
        }
        private struct JwtPayload
        {
            public long exp;
            public long iat;
            public string aud;
            public string iss;
            public string sub;
            public string email;
            public string email_verified;
        }
    }
}
