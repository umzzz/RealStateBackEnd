using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using RealStateAPI.Models;
using RealStateAPI.Service;

namespace RealStateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingController : ControllerBase
    {
        private readonly ListingService _listingService;

        public ListingController(ListingService listingService)
        {
            this._listingService = listingService;
        }

        [Route("{listingId:length(24)}")]
        [HttpGet]
        public async Task<ActionResult<ListingModel>> GetListing(string listingId)
        {
            var listing = await _listingService.GetByListingID(listingId);
            if (listing == null)
            {
                return NotFound();
            }
            else
            {
                Response.StatusCode = 201;
                return listing;
            }
        }
        [HttpPost]
        public async Task<ActionResult<string>> Post(ListingModel L)
        {
            //TODO : Validate Model 
            //https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-3.1

            try
            {
                var listing = await _listingService.GetByListingID(L.ListingID);
                if(listing == null)
                {
                    var id = await _listingService.Post(L);
                    Response.StatusCode = 201;
                    return Content($"resource with {id} was created");
                }
                else
                {
                    Response.StatusCode = 400;
                    return Content("Listing already exsists");
                }
            
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Content(ex.Message);

            }
        }
        [Route("filter")]
        [HttpPost]
        public async Task<ActionResult<List<ListingModel>>> Search(List<ListingFilter> filters)
        {
            var builder = Builders<ListingModel>.Filter;
            FilterDefinition<ListingModel> Monogfilter = FilterDefinition<ListingModel>.Empty;
            foreach (var filter in filters)
            {

                switch (filter.FilterName)
                {
                    case FilterKey.Price:

                        foreach (var item in filter.FilterValue)
                        {
                            var qty = Convert.ToDouble(item.Value);
                            if (item.Key.Equals("gt"))
                            {

                                Monogfilter &= builder.Gt(ListingModel => ListingModel.Price, qty);
                            }
                            if (item.Key.Equals("lt"))
                            {
                                Monogfilter &= builder.Lt(ListingModel => ListingModel.Price, qty);
                            }
                        }
                        break;
                    case FilterKey.NumberOfBedRooms:


                        if (filter.FilterValue.TryGetValue("eq", out var numofrooms))
                        {
                            
                            Monogfilter &= builder.Eq(ListingModel => ListingModel.BedProperties.NumberOfRooms, Convert.ToInt32(numofrooms));
                            break;
                        }
                        foreach (var item in filter.FilterValue)
                        {
                          
                            var qty = Convert.ToInt32(item.Value);

                            if (item.Key.Equals("gt"))
                            {

                                Monogfilter &= builder.Gt(ListingModel => ListingModel.BedProperties.NumberOfRooms, qty);
                            }
                            if (item.Key.Equals("lt"))
                            {
                                Monogfilter &= builder.Lt(ListingModel => ListingModel.BedProperties.NumberOfRooms, qty);
                            }

                        }
                        break;
                    default:
                        Response.StatusCode = 200;
                        return Content("Please select the correct Filter");

                }
            }
            var listing = await _listingService.Search(Monogfilter);
            if(listing.Count == 0)
            {
                Response.StatusCode = 200;
                return Content("We couldnt find any listings please expand your search");
            }
            Response.StatusCode = 200;
            return listing;
        }
    }
}