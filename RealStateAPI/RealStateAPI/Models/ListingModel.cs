using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateAPI.Models
{
    public class ListingModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string UserId { get; set; }
    }
}
