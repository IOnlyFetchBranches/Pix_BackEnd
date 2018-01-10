using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using pix_dtmodel.Models;

namespace pix_dtmodel
{
    public abstract class DataManager
    {


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
                picsSession = new Session<Pic>(connection, "pics");
                await picsSession.Add(filledModelPic);
                var test = picsSession.GetAll().Result.First();
                Console.WriteLine(test.Pid +" :pid    uid:"+test.Uid);

            }
        }
    }

}
