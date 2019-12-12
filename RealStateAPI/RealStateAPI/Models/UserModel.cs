using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateAPI.Models
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Listings { get; set; }
        public List<string> Liked { get; set; }
    }
}
