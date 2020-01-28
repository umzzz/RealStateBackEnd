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

        private readonly IS3Service _s3Service;

        public ListingController(ListingService listingService, IS3Service s3Service)
        {
            this._listingService = listingService;
            this._s3Service = s3Service;
        }

        [Route("{listingId:length(24)}")]
        [HttpGet]
        public async Task<ActionResult<ListingModel>> GetListing(string listingId)
        {
            var listing = await _listingService.GetByID(listingId);
            if (listing == null)
            {
                Response.StatusCode = 400;
                return Content("Listing Not found");
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
                if (listing == null)
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
        public async Task<ActionResult<List<ListingModel>>> Search(SearchModel search)
        {
            var builder = Builders<ListingModel>.Filter;
            FilterDefinition<ListingModel> Mongofilter = FilterDefinition<ListingModel>.Empty;
            foreach (var filter in search.Filters)
            {

                switch (filter.FilterName)
                {
                    case "Price":

                        foreach (var item in filter.FilterValue)
                        {
                            var qty = Convert.ToDouble(item.Value);
                            if (item.Key.Equals("gt"))
                            {

                                Mongofilter &= builder.Gt(ListingModel => ListingModel.Price, qty);
                            }
                            if (item.Key.Equals("lt"))
                            {
                                Mongofilter &= builder.Lt(ListingModel => ListingModel.Price, qty);
                            }
                        }
                        break;
                    case "NumberOfBedRooms":


                        if (filter.FilterValue.TryGetValue("eq", out var numofrooms))
                        {

                            Mongofilter &= builder.Eq(ListingModel => ListingModel.BedProperties.NumberOfRooms, Convert.ToInt32(numofrooms));
                            break;
                        }
                        foreach (var item in filter.FilterValue)
                        {

                            var qty = Convert.ToInt32(item.Value);

                            if (item.Key.Equals("gt"))
                            {

                                Mongofilter &= builder.Gt(ListingModel => ListingModel.BedProperties.NumberOfRooms, qty);
                            }
                            if (item.Key.Equals("lt"))
                            {
                                Mongofilter &= builder.Lt(ListingModel => ListingModel.BedProperties.NumberOfRooms, qty);
                            }

                        }
                        break;
                    case "NumberOfBathRooms":

                        if (filter.FilterValue.TryGetValue("eq", out var numofBathrooms))
                        {

                            Mongofilter &= builder.Eq(ListingModel => ListingModel.BathProperties.NumberOfRooms, Convert.ToInt32(numofBathrooms));
                            break;
                        }
                        foreach (var item in filter.FilterValue)
                        {

                            var qty = Convert.ToInt32(item.Value);

                            if (item.Key.Equals("gt"))
                            {

                                Mongofilter &= builder.Gt(ListingModel => ListingModel.BathProperties.NumberOfRooms, qty);
                            }
                            if (item.Key.Equals("lt"))
                            {
                                Mongofilter &= builder.Lt(ListingModel => ListingModel.BathProperties.NumberOfRooms, qty);
                            }
                        }
                        break;
                    default:
                        Response.StatusCode = 400;
                        return Content("Please select the correct Filter");
                }
            }
            if (!string.IsNullOrEmpty(search.SearchTerm))
            {
                Mongofilter &= builder.Or(builder.Regex(ListingModel => ListingModel.ListingID, new BsonRegularExpression(search.SearchTerm, "i")),
                    builder.Regex(ListingModel => ListingModel.Location.Address, new BsonRegularExpression(search.SearchTerm, "i")),
                    builder.Regex(ListingModel => ListingModel.Description, new BsonRegularExpression(search.SearchTerm, "i")));
            }
            if (!string.IsNullOrEmpty(search.PropertyType))
            {
                PropertyType propertyType = (PropertyType)Enum.Parse(typeof(PropertyType), search.PropertyType);
                Mongofilter &= builder.Eq(ListingModel => ListingModel.PropertyType, propertyType);
            }
            var listing = await _listingService.Search(Mongofilter);
            if (listing.Count == 0)
            {
                Response.StatusCode = 200;
                return Content("We couldnt find any listings please expand your search");
            }
            Response.StatusCode = 200;
            return listing;
        }
        [Route("Attachments")]
        public async Task<IActionResult> AddAttachments(ListingAttachmentModel Attachments)
        {
            List<Pictures> picturesToAdd = new List<Pictures>();
            ListingModel Listings = new ListingModel();
            if (!string.IsNullOrEmpty(Attachments.ListingId))
            {
                Listings = await _listingService.GetByID(Attachments.ListingId);
                if (Listings == null)
                {
                    Response.StatusCode = 400;
                    return Content("Listing Not found");
                }
                else
                {
                    picturesToAdd = Listings.Pictures;
                    foreach (string attachmentPath in Attachments.AttachmentPaths)
                    {
                        var pictureUrl = Guid.NewGuid().ToString();
                        picturesToAdd.Add(new Pictures { TypeID = PictureType.Featured, url = pictureUrl });
                        try
                        {
                            await _s3Service.uploadFile(pictureUrl, attachmentPath);
                        }
                        catch (Exception)
                        {

                            Response.StatusCode = 400;
                            return Content("We Could not upload all the pictures please try again");
                        }

                    }
                    UpdateDefinition<ListingModel> update = Builders<ListingModel>.Update.Set(l => l.Pictures, picturesToAdd);
                    FilterDefinition<ListingModel> Mongofilter = Builders<ListingModel>.Filter.Eq(l => l.Id, Attachments.ListingId);
                    try
                    {
                        await _listingService.UpdateListing(Mongofilter, update);
                    }
                    catch (Exception)
                    {

                        Response.StatusCode = 400;
                        return Content("We Could not upload all the pictures please try again");
                    }
                 
                    Response.StatusCode = 200;
                    return Content("All the attachmens Were Added");
                }
            }
            else
            {
                Response.StatusCode = 400;
                return Content("Please send in the listing ID");
            }

        }
    }
}