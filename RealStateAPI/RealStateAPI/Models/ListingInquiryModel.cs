using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateAPI.Models
{
    public class ListingInquiryModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string listingId { get; set; }
        public List<Inquiry> Inquiries { get; set; }
    }

    public class Inquiry
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string UserID { get; set; }
        public string EmailAddress { get; set; }
        public string Message { get; set; }
        public DateTime DateSent { get; set; } = DateTime.UtcNow;
    }
}
