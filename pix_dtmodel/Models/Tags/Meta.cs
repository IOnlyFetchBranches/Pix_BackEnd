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
        

        class Geo
        {
            

        }

        class Date
        {

            
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
