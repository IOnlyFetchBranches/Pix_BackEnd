using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace pix_dtmodel.Models
{
    public class Session <T> where T:class
    {

        //Our dbase instance
        private IMongoDatabase database;
        //Our collections
        private IMongoCollection<T> collection;

        private readonly string collectionName;
        public string Collection => collectionName;

        

        //Constructor initializes a new session with mongo
        public Session(IMongoDatabase connection, string collectionName)
        {
            Debug.Assert(connection != null);
            //Assign connection
            database = connection;

            //Update readable name
            this.collectionName = collectionName;

            //Open collection
           collection= database.GetCollection<T>(collectionName);

    
        }

        public async Task<T> GetRecordById(string id)
        {
            var item = await collection.Find(Builders<T>.Filter.Eq("_id", id)).SingleAsync();


            return item;


        }

        public async Task<IQueryable<T>> GetAll()
        {
            var results = await collection.Find(_ => true).ToListAsync();
            
            return results.AsQueryable();
        }

        public async Task<string> Add(T model)
        {
            await collection.InsertOneAsync(model);
            return "success!";
        }

        public async Task<string> remove(string id)
        {
            await collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
            return "success!";
        }




    }
}
