using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace pix_dtmodel.Models
{
    public class Meta
    {
        //This class defines meta data fields
        [BsonElement("_id")]
        public string Pid { get; set; }

        class Geo
        {
            //Geotag model

           
            //Mark the type
            public readonly string Type = "geo";

            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }

        }

        class Date
        {

            //Mark the type
            public readonly string Type = "date";

            class Modified
            {
                public string Day { get; set; }
                public string Month { get; set; }
                public string Year { get; set; }
            }

            class Created
            {
                public string Day { get; set; }
                public string Month { get; set; }
                public string Year { get; set; }
            }
        }

        //May not be populated depending on compatibility
        class Hardware
        {
            public string Os { get; set; }
            public string Model { get; set; }
            public string Manufacturer { get; set; }
            
        }
    }
}
