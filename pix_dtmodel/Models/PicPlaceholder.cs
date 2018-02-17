using System;
using System.Dynamic;
using MongoDB.Bson.Serialization.Attributes;

namespace pix_dtmodel.Models
{
    public class PicPlaceholder : Pic
    {
        [BsonIgnore] private bool assigned; // control to note that original fields have already been assigned, [Bootleg readonly]
        [BsonIgnore] private int EXPIRY_CONSTANT = 5; //Amount of minutes that a placeholder is  valid!

        private   DateTime created, expires;

        public DateTime Created
        {
            get { return created; }
            set
            {
                if (!assigned)
                {
                    created = DateTime.Now;
                    assigned = true;
                }
               
            }
        }

        public DateTime Expires
        {
            get => expires;
            set
            {
                if (!assigned)
                {
                    expires = created.AddMinutes(EXPIRY_CONSTANT);
                    assigned = true;
                }

            }
        }



    }
}