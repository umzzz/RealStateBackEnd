using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            var listing = await _listingService.GetByID(listingId);
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
    }
}