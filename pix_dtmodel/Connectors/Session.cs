using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver.Linq;
using pix_dtmodel.Adapters;
using pix_dtmodel.Models;
using pix_dtmodel.Util;
using static pix_dtmodel.Util.DtLogger;

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

        //Getters
        public IMongoCollection<T> GetCollection()
        {
            return collection;
         }

        public IMongoDatabase GetDatabase()
        {
            return database;
        }

        

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

        public async Task<UpdateResult> UpdateRecordById(string id,string fieldToUpdate, IUpdatable newState)
        {
            try
            {
                var filter = Builders<T>.Filter.Eq("_id", id);

                var action = Builders<T>.Update.Set(
                    fieldToUpdate, (string) newState.getBundle()[fieldToUpdate]
                );

                LogG("Session", "Value to add: " +(string) newState.getBundle()[fieldToUpdate]);


                var result = await collection.UpdateOneAsync(filter, action);




                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Caught Error, Could not find item [GetRecordById]\n"+
                                e.Message);
                LogG("Session","Could not find item to update " + id + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
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
            LogG("Session", " Pushing...");

            var result = collection.InsertOneAsync(model);
            
            if (result.Exception != null)
            {
                LogG("Session", " Push error - " + result.Exception.Message);
            }
            await result;
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
                        LogG("Session", "No Match for " + key + " With Matching value " + valueBeingCompared);
                        return false;
                    }

                LogG("Session", "Found Document, Verifying Value..." );
                bool good = find.ToBsonDocument().ToDictionary().ContainsValue(valueBeingCompared);
                LogG("Session", "Done... ["+good+"]");
                return good;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Caught Error, Possibly Wrong Username? \n" + e.Message);
                LogG("Session", e.Message);
                return false;
            }

        }


        //Geo Methods [Cannot be Generic unfortunately :(]

            //Control


        public async  Task<IQueryable<T>> GetPicsByUserWithin(string uid, GeoJsonPoint<GeoJson2DGeographicCoordinates> point, double maxDist, Session<Pic> session)
        {

            

            //Do index [if needed]
            IndexFactory.GetPicIndexer(session).MakeIndex();    
            //Build Query
            var query = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq("_id", uid),
                Builders<T>.Filter.Near("GeoData", point, maxDist));

            //Run Query
            var results = (await collection.FindAsync(query)).Current;

            //Log
            LogG("Session", "query type-WithinGeo/Id size-" + results.Count() + " \n requestBy-" +uid+" " +
                            "centered-"+ point.Coordinates.Latitude +":"+point.Coordinates.Longitude);


            return results.AsQueryable();




        }

        public static T GetByUserAt(string uid)
        {
            throw new NotImplementedException("This method is deprecated/not in use!");
        }





        


    }


}
