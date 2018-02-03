﻿using System;
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
        public string Gid { get; set; }
        public string Uid { get; set; }
    }
}
