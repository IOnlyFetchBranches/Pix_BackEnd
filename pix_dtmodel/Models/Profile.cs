using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace pix_dtmodel.Models
{
    //Transfer Safe Version of User, That is filled by query request.
    public class Profile
    {
        [BsonElement("_id")]
        [BsonDefaultValue("Bad Context")]
    
        public string Uid { set; get; }

        public string Username { get; set; }

        private string email;
        public string Email
        {
            get => "Bad Context";
            set => email = "Ignored!";
        }

        [BsonIgnoreIfNull]
        public string First { get; set; }

        [BsonIgnoreIfNull]
        public string Last { get; set; }

        [BsonIgnoreIfNull]
        public bool Verified { get; set; }


        

        private string gid;

        public string Gid
        {
            get => "Bad Context";
            set => gid = "Ignored";
        }

       
       

       

    }
}


