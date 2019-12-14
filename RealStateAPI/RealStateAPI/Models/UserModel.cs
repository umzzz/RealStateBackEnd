using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateAPI.Models
{
    public class UserModel : MongoUser
    {
        public List<string> Listings { get; set; }
        public List<string> Liked { get; set; }
    }

    public class AddRoleModel
    {
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
    }

    public static class Role
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }


}
