namespace RealStateAPI.Models
{
    public interface IUserDataBaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string UserCollectionName { get; set; }
    }
    public interface IListingDataBaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string ListingCollectionName { get; set; }
    }
    public interface IListingInquiryDataBaseModel
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string ListingInquiryCollectionName { get; set; }
    }
}