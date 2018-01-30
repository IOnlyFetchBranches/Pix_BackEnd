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
        public string Token { get; set; }
        public string Location { get; set; }
        //Set name
        public string Name { get; set; }

        //End of Required Fields.




        //optional may not always be supplied to the api ["city","state"] or ["street","city","state]
        //Geotag model


        //Mark the type
        //GeoData

        [BsonIgnoreIfNull]
        public string Lon { get; set; }
        [BsonIgnoreIfNull]
        public string Lat { get; set; }

        [BsonIgnoreIfNull]
        public string Street { get; set; }
        [BsonIgnoreIfNull]
        public string City { get; set; }
        [BsonIgnoreIfNull]
        public string State { get; set; }
        [BsonIgnoreIfNull]
        public string Country { get; set; }

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









    }
}
