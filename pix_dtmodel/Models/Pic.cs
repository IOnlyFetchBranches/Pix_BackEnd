using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver.GeoJsonObjectModel;
using pix_dtmodel.Adapters;
using pix_dtmodel.Connectors;


namespace pix_dtmodel.Models
{
    /**
     * This is the model class for a pic
     * Implements IUpdatable so that session can retrieve values from it without knowing the type via a dictionary.
     */
    public class Pic : IUpdatable
    {

        //Contains fields for pic id, user id, storage location, date_created, geotags
        [BsonElement("_id")] //Pid needs to be our indexed field
        public string Pid{ get; set;}
        public string Uid { get; set; }
        public string Gid { get; set; }
        [BsonIgnoreIfNull]
        public string Description { get; set; }
        
        public string Location { get; set; }
        //Set name
        public string Name { get; set; }

        //End of Required Fields for database

        //Data payload [Filled by controller i/o at time of request]
        [BsonIgnore]
        public string B64Data { get; set; }
        
        /// <summary>
        /// Check this to see if it's been uploaded already.
        /// </summary>
        public bool Filled { get; set; }




        //optional may not always be supplied to the api ["city","state"] or ["street","city","state]
        //Geotag model

        [BsonElement("geodata")]
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> GeoData { get; set; }


        //Mark the type
        // Old method for geo data

        [BsonIgnoreIfNull]
        public string Lon { get; set; }
        [BsonIgnoreIfNull]
        public string Lat { get; set; }

        //Set all date fields from a single date object.
        [BsonIgnore]
        public DateTime CreationDate
        {
            get => DateTime.Parse(CreationYear+"/"+CreationMonth+"/"+CreationDay+" "+ CreationHour +":"+CreationMin+":"+CreationSec);
            set
            {
                CreationHour = value.Hour + "";
                CreationMin = value.Minute + "";
                CreationSec = value.Second + "";
                CreationYear = value.Year + "";
                CreationMonth = value.Month + "";
                CreationDay = value.Day + "";
            }
        }

        //Set all date fields from a single date object.
        [BsonIgnore]
        public DateTime ModifiedDate
        {
            get => DateTime.Parse(CreationYear + "/" + CreationMonth + "/" + CreationDay + " " + ModHour + ":" + ModMin + ":" + ModSec);
            set
            {
                ModHour = value.Hour + "";
                ModMin = value.Minute + "";
                ModSec = value.Second + "";
                ModYear = value.Year + "";
                ModMonth = value.Month + "";
                ModDay = value.Day + "";
            }
            
        }


        [BsonIgnoreIfNull]
        public string ModDay { get; set; }
        [BsonIgnoreIfNull]
        public string ModMonth { get; set; }
        [BsonIgnoreIfNull]
        public string ModYear { get; set; }
        [BsonIgnoreIfNull]
        public string ModHour { get; set; }
        [BsonIgnoreIfNull]
        public string ModMin { get; set; }
        [BsonIgnoreIfNull]
        public string ModSec { get; set; }


        [BsonIgnoreIfNull]
        public string CreationDay { get; set; }
        [BsonIgnoreIfNull]
        public string CreationMonth { get; set; }
        [BsonIgnoreIfNull]
        public string CreationYear { get; set; }
        [BsonIgnoreIfNull]
        public string CreationHour { get; set; }
        [BsonIgnoreIfNull]
        public string CreationMin { get; set; }
        [BsonIgnoreIfNull]
        public string CreationSec { get; set; }


        [BsonIgnore] private IDictionary<string, object> dataBundle;

        
        public IDictionary<string, object> getBundle()
        {
            if (dataBundle == null)
            {
                //Bit of trickery here... [Be careful!]
                var dataBundle = this.ToBsonDocument(new BsonClassMapSerializer<Pic>(BsonClassMap.LookupClassMap(this.GetType())));

                this.dataBundle = dataBundle.ToDictionary();




                return this.dataBundle;
            }
            else
            {
                return dataBundle;
            }
        }
    }
}
