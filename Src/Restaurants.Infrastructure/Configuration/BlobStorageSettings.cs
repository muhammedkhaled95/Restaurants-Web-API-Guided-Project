namespace Restaurants.Infrastructure.Configuration;

internal class BlobStorageSettings
{
    public string ConnectionString { get; set; } = default!;
    public string LogosContainerName { get; set; } = default!;
    public string AccountKey { get; set; } = default!;


}
