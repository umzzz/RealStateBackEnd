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
        private readonly IMongoCollection<ListingModel> _listing;

        public ListingService(IListingDataBaseSettings listingSettings)
        {
            var client = new MongoClient(listingSettings.ConnectionString);
            var dataBase = client.GetDatabase(listingSettings.DatabaseName);
            _listing = dataBase.GetCollection<ListingModel>(listingSettings.ListingCollectionName);
        }

        public async Task<ListingModel> GetByListingID(string id)
        {
            ListingModel l = await _listing.Find(x => x.ListingID == id).FirstOrDefaultAsync();
            return l;
        }

        public async Task<ListingModel> GetByID(string id)
        {
            ListingModel l = await _listing.Find(x => x.Id == id).FirstOrDefaultAsync();
            return l;
        }
        public async Task<string> Post(ListingModel L)
        {
            await _listing.InsertOneAsync(L);

            return L.Id;
        }

        public async Task<List<ListingModel>> Search(FilterDefinition<ListingModel> filter)
        {
            if(filter == null)
            {
                return new List<ListingModel>();
            }
            var listings = await  _listing.FindAsync(filter);
            return await listings.ToListAsync();
        }

        public async Task<bool> UpdateListing(FilterDefinition<ListingModel> filter,  UpdateDefinition<ListingModel> Pictures )
        {
            if(filter != null && Pictures != null)
            {
                UpdateOptions options = new UpdateOptions
                {
                    IsUpsert = true
                };
                var result = await _listing.UpdateOneAsync(filter, Pictures, options);

                return result.IsAcknowledged;
            }
            else
            {
                return false;
            }
        }
    }
}
