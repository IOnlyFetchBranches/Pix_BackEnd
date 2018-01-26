using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace pix_dtmodel.Models
{
    public class User
    {
        //Supp Classes
        public class Name
        {

            //First, Last, Additional properties
            public string First { get; set; }
            public string Last { get; set; }

            [BsonIgnoreIfNull]
            public string[] Additional { get; set; }
        }

        [BsonElement("_id")]
        public string Uid { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }
        
        public string HashWord { get; set; }

        public string Token { get; set; }

        public string Status { get; set; }

        [BsonIgnoreIfNull]
        public bool IsBanned { get; set; }
        [BsonIgnoreIfNull]
        public string BanReason { get; set; }
        [BsonIgnoreIfNull]
        public string TimeLeft { get; set; }

        [BsonIgnoreIfNull]
        public string Gid { get; set; } //Google id


    

        



        
    }
}
