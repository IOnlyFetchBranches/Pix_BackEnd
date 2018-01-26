using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pix_dtmodel.Connectors;
using pix_dtmodel.Managers;

namespace pix_dtmodel.Models
{
    public class Defaults
    {
        public class ConnectionStrings
        {
            public static string Localhost => "mongodb://localhost";
        }

        public class DatabaseNames
        {
            public static string TestProd => "admin";
        }

        public class Collections
        {
            public static string Meta => "pix_meta";

            public static string Pics => "pix_pics";

            public static string Users => "pix_users";

            public static string SessionData => "pix_sessions";
        }

        public class System
        {
            private static string apikey;
            public static string ApiKey
            {
                get
                {
                    if (apikey == null)
                    {
                        apikey = new Session<SystemRecord>(
                            DataManager.Init(Defaults.ConnectionStrings.Localhost, Defaults.DatabaseNames.TestProd,
                                null), "pix_sys").GetRecordById("pix_google_apikey").Result.Data;

                        return apikey;

                    }
                    else
                    {
                        return apikey;
                    }

                }
            }
        }
    }
}
