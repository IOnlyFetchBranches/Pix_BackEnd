using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using pix_dtmodel.Adapters;
using pix_dtmodel.Connectors;
using pix_dtmodel.Models;

namespace pix_dtmodel.Util
{
    public  class IndexFactory
    {
        //Handles Indexing Involving Class Specific Fields that cannot be done 


        public static  IIndexer GetPicIndexer(Session<Pic> session)
        {
            return PicIndexer.Get(session);

        }

    
        private class PicIndexer : IIndexer
        {

            private readonly IMongoCollection<Pic> collection;
            

             PicIndexer(Session<Pic> picSession)
            {
                collection = picSession.GetCollection();
            }


            public void MakeIndex()
            {
                //Index here
                collection.Indexes.CreateOne(Builders<Pic>.IndexKeys.Geo2DSphere(x => x.GeoData));
            }

            public static IIndexer Get(Session<Pic> picSession)
            {
                return new PicIndexer(picSession);
            }
        
            
        }
    }

    
}
