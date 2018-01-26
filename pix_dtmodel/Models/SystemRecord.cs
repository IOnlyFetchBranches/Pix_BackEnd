using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace pix_dtmodel.Models
{
    //Used to retrieve secured system data, such as api keys from the database
    [Serializable]
    public class SystemRecord
    {
        [BsonElement("_id")]
        public string Type { get; set; }
        [BsonElement("data")]
        public string Data { get; set; }

        [Serializable]
        public class Cred
        {
            [BsonElement("_id")]
            public string Username { get; set; }
            [BsonElement("hashword")]
            public string Hashword { get; set; }
            [BsonElement("role")]
            public string Role { get; set; }

        }
    }
}
