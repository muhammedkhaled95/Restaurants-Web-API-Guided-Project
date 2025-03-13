namespace Restaurants.Infrastructure.Configuration;

internal class BlobStorageSettings
{
    public String ConnectionString { get; set; } = default!;
    public String LogosContainerName { get; set; } = default!;
}
