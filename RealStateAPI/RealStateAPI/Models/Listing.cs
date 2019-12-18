using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateAPI.Models
{
    public class Listing
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        [Required]
        public string ListingID { get; set; }
        public string Description { get; set; }
        public Location Location { get; set; }
        public PropertyType PropertyType { get; set; }
        public string PropertySubType { get; set; }
        public double Price { get; set; }
        public List<Dictionary<string,string>> BedProperties { get; set; }
        public List<Dictionary<string, string>> BathProperties { get; set; }
        public Dictionary<string, List<PropertyFeatures>> PropetyProperties { get; set; }
        public int Buildyear { get; set; }
        public Dictionary<PictureType,string> Pictures { get; set; }
        public string Fees { get; set; }
    }

    public enum PropertyType
    {
        Comercial,
        resedential
    }
    public enum PictureType
    {
        Featured,
        NonFeatured
    }

    public class Location
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
    }

    public class PropertyFeatures
    {
        public Dictionary<string,string> PropertyFeature { get; set; }
    }

}
