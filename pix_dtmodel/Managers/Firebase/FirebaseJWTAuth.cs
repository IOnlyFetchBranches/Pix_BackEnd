using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    using Util;

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

        private static string CurrentB64;
        public async Task<Dictionary<string,string>> Verify(string token)
        {
            try
            {
                //Setup Logging
                //Create Dir if it doesnt exist
                Directory.CreateDirectory("C:\\DebugLogs\\Verification\\");

                //Open LogFile
                var LogFile = new FileStream("C:\\DebugLogs\\Verification\\" + "log" + DateTime.Now.ToString("MM-dd-yyyy") + "-"
                                             + DateTime.Now.ToString("h-mm-ss") + " " + DateTime.Now.Millisecond + ".txt",
                    FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);




                //following instructions from https://firebase.google.com/docs/auth/admin/verify-id-tokens
                string hashChunk = token; //keep for hashing later on
                hashChunk = hashChunk.Substring(0, hashChunk.LastIndexOf('.'));
                token = token.Replace('-', '+').Replace('_', '/'); //sanitize for base64 on C#
                DtLogger.Log("Sanitization [1]",LogFile);
                string[] sections = token.Split('.'); //split into 3 sections according to JWT standards
                DtLogger.Log("Split...", LogFile);
                JwtHeader header = B64Json<JwtHeader>(sections[0]);

                DtLogger.Log("Got Header.", LogFile);


                if (header.alg != "RS256")
                {
                    DtLogger.Log("Incorrect Alg In Header!", LogFile);
                    LogFile.Dispose();
                    return null;
                }

                DtLogger.Log("Header Good.", LogFile);

                DtLogger.Log("Pulling Keys...", LogFile);
                HttpResponseMessage res = await Req.GetAsync("x509/securetoken@system.gserviceaccount.com");
                DtLogger.Log("Done!", LogFile);
                string keyDictStr = await res.Content.ReadAsStringAsync();
                DtLogger.Log("Reading keys...", LogFile);
                Dictionary<string, string> keyDict =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(keyDictStr);
                DtLogger.Log("Done.", LogFile);
                string keyStr = null;
                keyDict.TryGetValue(header.kid, out keyStr);
                if (keyStr == null)
                {
                    DtLogger.Log("No Matching Key Was Found...", LogFile);
                    LogFile.Dispose();
                    return null;
                }

                DtLogger.Log("Processing Challenge...", LogFile);
                using (var rsaCrypto = CertFromPem(keyStr))
                {
                    using (var hasher = SHA256Managed.Create())
                    {   
                        
                        byte[] plainText = Encoding.UTF8.GetBytes(hashChunk);
                        byte[] hashed = hasher.ComputeHash(plainText);

                        DtLogger.Log("Decoding...", LogFile);
                        byte[] challenge = SafeB64Decode(sections[2]);
                        DtLogger.Log("Done.", LogFile);

                        RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsaCrypto);
                        rsaDeformatter.SetHashAlgorithm("SHA256");
                        if (!rsaDeformatter.VerifySignature(hashed, challenge))
                        {
                            DtLogger.Log("Signature Verification Failed!", LogFile);
                            return null;
                        }
                    }
                }

                JwtPayload payload = B64Json<JwtPayload>(sections[1]);
                long currentTime = EpochSec();
                //the if chain of death
                if (payload.aud != FirebaseId ||
                    payload.iss != "https://securetoken.google.com/" + FirebaseId ||
                    //Was checking for >= but on super fast days you could get an iat = to the server date
                    //Transfer from google to my server happens under a second and invalidates a good token.
                    payload.iat > currentTime ||
                    payload.exp <= currentTime)
                {
                    DtLogger.Log("Payload verification failure.", LogFile);
                    DtLogger.Log(
                        "Condition checks [aud,iss,iat,exp] : \n"
                        + (payload.aud != FirebaseId) +"\n" 
                        + (payload.iss != "https://securetoken.google.com/" + FirebaseId )+ "\n"
                        + (payload.iat >= currentTime)  +" [" +payload.iat + "? >" + currentTime +"]" + "\n"
                        + (payload.exp <= currentTime), LogFile);
                    LogFile.Dispose();
                    return null;
                }




                DtLogger.Log("Verification success, attempt to get gid: \n\t" +payload.sub, LogFile);
                //Grab payload details and return.

                
                DtLogger.Log("Attempt to create response package...", LogFile);
                Dictionary<string, string> dict = new Dictionary<string, string>();

                DtLogger.Log("Adding gid...", LogFile);
                dict.Add("gid", payload.sub);
                DtLogger.Log("Adding email...", LogFile);
                dict.Add("email", payload.email);
                DtLogger.Log("Adding status...", LogFile);
                dict.Add("email_verified", payload.email_verified);

                DtLogger.Log("Done.", LogFile);

                DtLogger.Log("Returning! dict exists = " + (dict != null), LogFile);
                
                LogFile.Dispose();
                return dict;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message + "\n" +CurrentB64);
            }


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
                    CurrentB64 = newEncodedPad+ " Pad";
                    return Convert.FromBase64String(newEncodedPad);
                }
                CurrentB64 = newEncoded +" NonPad";
                return Convert.FromBase64String(newEncoded);
            }

            if (encoded.Contains("data="))
            {
                var newEncoded =
                    encoded.Replace("data=",
                        ""); // encoded.Replace("\r\n", "") + new string('=', 4 - encoded.Length % 4)
                if (newEncoded.Length % 4 != 0)
                {
                    string newEncodedPad =
                        newEncoded + new string('=', 4 - newEncoded.Length % 4); //'=', 4 - encoded.Length % 4
                    CurrentB64 = newEncodedPad + " Data = caught  + Pad";
                    return Convert.FromBase64String(newEncodedPad);
                }
                CurrentB64 = newEncoded + "Data = caught but NonPad";
                return Convert.FromBase64String(newEncoded);
            }



            if (encoded.Length % 4 != 0)
                {
                    string encodedPad =
                        encoded + new string('=', 4 - encoded.Length % 4); //'=', 4 - encoded.Length % 4
                    CurrentB64 = encodedPad + " NonBadEnd Pad";
                return Convert.FromBase64String(encodedPad);
                }

            // Console.WriteLine(encodedPad);
            CurrentB64 = encoded + " NonBadEnd NonPad";
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
