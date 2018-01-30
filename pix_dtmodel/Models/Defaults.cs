using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
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

        public class Fields
        {
            public class Users
            {
                //Returns field names for users fields

                public static string Uid => "uid";

                public static string Username => "Username";

                //First, Last, Additional properties
                public static string First => "First";
                public static string Last => "Last";


                public static string Additional => "Additional";

                public static string Email => "Email";

                public static string HashWord => "Hashword";

                public static string Token => "Token";

                public static string Status => "Status";

               
                public static string IsBanned => "IsBanned";
             
                public static string BanReason => "BanReason";

                public static string TimeLeft => "TimeLeft";


                public static string Gid => "Gid"; //Google id

            }
        }
        //Directories, Keys,etc
        public class System
        {
            private static string apikey;
            //Dynamically gets ApiKey From Mongod
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

            //FilePaths

            public static string HtmlRoot => "C:\\Pix\\Pages";

            public static string PixRoot => "C:\\Pix";

            public static string UserRoot => "C:\\Pix\\Users";


        }
    }
}
