using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;



namespace pix_dtmodel.Models
{
    /**
     * This is the model class for a pic
     */
    public class Pic
    {
        //Contains fields for pic id, user id, storage location, date_created, geotags
        [BsonElement("_id")] //Pid needs to be our indexed field
        public string Pid{ get; set;}

        public string Uid { get; set; }
        public string Location { get; set; }
    



        //optional may not always be supplied to the api ["city","state"] or ["street","city","state]
        [BsonIgnoreIfNull]
        private Meta Metadata { get; set; }


       

      

        

    }
}
