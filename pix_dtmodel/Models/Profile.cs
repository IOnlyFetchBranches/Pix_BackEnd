using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace pix_dtmodel.Models
{
    //Transfer Safe Version of User, That is filled by query request.
    public class Profile
    {
        [BsonElement("_id")]
        public string UID { get; set; }
        public class Name
        {

            //First, Last, Additional properties
            public string First { get; set; }
            public string Last { get; set; }

            [BsonIgnoreIfNull]
            public string[] Additional { get; set; }
        }

        public string Username { get; set; }

        private string email;
        public string Email
        {
            get => "Bad Context";
            set => email = "Ignored!";
        }

        private string hashword;

        public string HashWord
        {
            get => "Bad Context";
            set => hashword = "Ignored!";
        }

        private string token;

        public string Token
        {
            get => "Bad Context";
            set => token = "Ignored";
        }

        public string Status { get; set; }

        private bool isBanned = true;

        [BsonIgnoreIfNull]
        public bool IsBanned
        {
            get => isBanned;
            set => isBanned = value;
        }

        [BsonIgnoreIfNull]
        public string BanReason { get; set; }
        [BsonIgnoreIfNull]
        public string TimeLeft { get; set; }

        [BsonIgnoreIfNull]
        public string Gid { get; set; } //Google id

    }
}


