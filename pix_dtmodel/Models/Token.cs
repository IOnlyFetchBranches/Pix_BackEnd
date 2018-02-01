using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace pix_dtmodel.Models
{
    public class SessionToken
    {
        [BsonElement("_id")]
        public string Token { get; set; }

        public string Uid { get; set; }

        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        
    }
}
