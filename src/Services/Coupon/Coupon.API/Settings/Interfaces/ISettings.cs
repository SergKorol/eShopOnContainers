namespace Coupon.API.Interfaces;

public interface ISettings
{
    string ConnectionString { get; set; }

    string MongoDatabase { get; set; }

    string EventBusConnection { get; set; }

    bool UseCustomizationData { get; set; }

    bool AzureStorageEnabled { get; set; }
}