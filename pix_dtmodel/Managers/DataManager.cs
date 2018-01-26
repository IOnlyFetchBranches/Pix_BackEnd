using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using pix_dtmodel.Connectors;
using pix_dtmodel.Models;

namespace pix_dtmodel.Managers
{
    public abstract class DataManager
    {
        /*
         * This class is the master method holder, that interacts with a Connector. In this case that is Session.
         * The connector contains a bunch of pseudo methods of interacing wiht the backend, given a certain input.
         * It's all generic and depends on the MODEL class that you pass to the session constructor.
         */

        public static IMongoDatabase Init(string connectionString, string dbName, MongoDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(connectionString); //Create the client;
            //Will ahve some additional overhead later :) But for testing just return the client

            //Done
            return client.GetDatabase(dbName, settings);
        }








        public class Testing
        {
            private static IMongoDatabase connection;
            private static Session<pix_dtmodel.Models.Pic> picsSession;

            public static async void addAndDisplayPic(Pic filledModelPic)

            {
                connection = DataManager.Init("mongodb://localhost", "admin", null);
                picsSession = new Session<Pic>(connection, "pix_pics");
                await picsSession.Add(filledModelPic);
                var testQ = picsSession.GetAll();
                {
                    foreach (var test in testQ.Result)
                    {
                        Console.WriteLine(test.Pid + " :pid    uid:" + test.Uid);
                    }

                }
            }
        }

    }
}
