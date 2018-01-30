using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using pix_dtmodel.Models;

namespace pix_dtmodel.Connectors
{
    public class Session <T> where T:class
    {

        //Our dbase instance
        private IMongoDatabase database;
        //Our collections
        private IMongoCollection<T> collection;

        private readonly string collectionName;
        public string Collection => collectionName;

        public EventHandler onQueryComplete;

        

        //Constructor initializes a new session with mongo
        public Session(IMongoDatabase connection, string collectionName)
        {
            Debug.Assert(connection != null);
            //Assign connection
            database = connection;

            //Update readable name
            this.collectionName = collectionName;

            Debug.WriteLine("Opening Session "+collectionName);

            //Open collection
           collection= database.GetCollection<T>(collectionName);
            
            Debug.WriteLine("Established!");

    
        }

        public async Task<T> GetRecordById(string id)
        {
            try
            {
                var item = await collection.Find(Builders<T>.Filter.Eq("_id", id)).SingleAsync();
                return item;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Caught Error, Could not find item [GetRecordById]\n"+
                                e.Message);
                return null;
            }


        }

        //Get s all members and returns a list
        public async Task<IQueryable<T>> GetAll()
        {
            Debug.WriteLine("Possibly Time Consuming Request <FindAll> ");
            var results = await collection.Find(_ => true).ToListAsync();
            Debug.WriteLine("Query returned "+ results.Count +" results!");
           
                return results.AsQueryable();
        }

        public async Task<IQueryable<T>> GetAll(int limit)
        {
            Debug.WriteLine("Possibly Time Consuming Request <FindAll> ");
            var results = await collection.Find(_ => true).Limit(limit).ToListAsync();
            Debug.WriteLine("Query returned " + results.Count + " results!");
      
                return results.AsQueryable();
        }

        public async Task<IQueryable<T>> GetSomeById(string id,int limit)
        {
            Debug.WriteLine("Possibly Time Consuming Request <FindAll> ");
            var results = await collection.Find(Builders<T>.Filter.Eq("_id", id)).Limit(limit).ToListAsync();
            Debug.WriteLine("Query returned " + results.Count + " results!");
            
                return results.AsQueryable();
        }

        public async Task<IQueryable<T>> GetSomeByIdAfterOffset(string id, int offset, int limit)
        {
            Debug.WriteLine("Possibly Time Consuming Request <FindAll> ");
            var results = await collection.Find(Builders<T>.Filter.Eq("_id", id))
                .Skip(offset).Limit(limit).ToListAsync();
            Debug.WriteLine("Query returned " + results.Count + " results!");
            if (results.Count == 0)
            {
                return null;
            }
            else
                return results.AsQueryable();
        }
        public async Task<IQueryable<T>> GetSomeAfterOffset(int offset, int limit)
        {
            Debug.WriteLine("Possibly Time Consuming Request <FindAll> ");
            var results = await collection.Find(_ => true)
                .Skip(offset).Limit(limit).ToListAsync();
            Debug.WriteLine("Query returned " + results.Count + " results!");
            if (results.Count == 0)
            {
                return null;
            }
            else
                return results.AsQueryable();
        }

        public async Task<IQueryable<T>> SortedGetSomeById(string id, string fieldToSortBy, int limit )
        {
            
            Debug.WriteLine("Possibly Time Consuming Request <FindAll> ");
            var results = await collection.Find(Builders<T>.Filter.Eq("_id", id))
                .Sort(Builders<T>.Sort.Ascending(fieldToSortBy))
                    .Limit(limit).ToListAsync();
            Debug.WriteLine("Query returned " + results.Count + " results!");
            
                return results.AsQueryable();
        }

        public async Task<string> Add(T model)
        {
            await collection.InsertOneAsync(model);
            return "success!";
        }

        public async Task<string> Remove(string id)
        {
            await collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
            return "success!";
        }

        public async Task<bool> CheckFieldFrom(string key, string fieldToCheck, string valueBeingCompared)
        {
            try
            {
                T find = await collection.Find(Builders<T>.Filter.Eq("_id", key)).SingleAsync();
           
            
            
                    if (find == null)
                    {
                        return false;
                    }   
                bool good = find.ToBsonDocument()[fieldToCheck] == valueBeingCompared;
                return good;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Caught Error, Possibly Wrong Username? \n" + e.Message);
                return false;
            }

        }




    }
}
