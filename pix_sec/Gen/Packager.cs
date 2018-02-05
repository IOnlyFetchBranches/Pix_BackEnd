using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace pix_sec.Gen
{
    //Static class for wrapping json
    public abstract class Packager
    {

        public static string CreateAuthPackage(IDictionary<string, string> verifiedToken, FileStream logFile)
        {
            if (logFile != null)
            {

                Dictionary<string, string> authPackage = new Dictionary<string, string>();
                
                authPackage.Add("gid", verifiedToken["gid"]);
                authPackage.Add("uid", Gen.ID.GenUid(verifiedToken["email"]));
                authPackage.Add("email", verifiedToken["email"]);
                authPackage.Add("email_verified", verifiedToken["email_verified"]);

                return JsonConvert.SerializeObject(authPackage);

            }
            else
            {
                Dictionary<string, string> authPackage = new Dictionary<string, string>();

                authPackage.Add("gid", verifiedToken["gid"]);
                authPackage.Add("uid", Gen.ID.GenUid(verifiedToken["email"]));
                authPackage.Add("email", verifiedToken["email"]);
                authPackage.Add("email_verified", verifiedToken["email_verified"]);

                return JsonConvert.SerializeObject(authPackage);
            }
        }
    }
}
