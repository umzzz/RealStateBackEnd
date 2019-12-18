using Microsoft.AspNetCore.Components;
using MongoDB.Driver;
using RealStateAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealStateAPI.Service
{
    public class ListingService
    {
        private readonly IMongoCollection<Listing> _listing;

        public ListingService(IListingDataBaseSettings listingSettings)
        {
            var client = new MongoClient(listingSettings.ConnectionString);
            var dataBase = client.GetDatabase(listingSettings.DatabaseName);
            _listing = dataBase.GetCollection<Listing>(listingSettings.ListingCollectionName);
        }

        public async Task<Listing> GetByID(string id)
        {
            Listing l = await _listing.Find(x => x.Id == id).FirstOrDefaultAsync();
            return l;
        }

        public async Task<Listing> GetByListingID(string id)
        {
            Listing l = await _listing.Find(x => x.ListingID == id).FirstOrDefaultAsync();
            return l;
        }


        public async Task<string> Post(Listing L)
        {
            await _listing.InsertOneAsync(L);

            return L.Id;
        }
    }
}
