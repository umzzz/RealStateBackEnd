using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateAPI.Models
{
    public class UserDataBaseSettings : IUserDataBaseSettings
    {
        public string UserCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }



    public class ListingDataBaseSettings : IListingDataBaseSettings
    {
        public string ListingCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }



    public class ListingInquiryDataBaseModel : IListingInquiryDataBaseModel
    {
        public string ListingInquiryCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
