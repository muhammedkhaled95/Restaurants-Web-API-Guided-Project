using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Interfaces;
using Restaurants.Infrastructure.Configuration;

namespace Restaurants.Infrastructure.Storage;

/// <summary>
/// Service for handling blob storage operations in Azure Blob Storage.
/// Implements the <see cref="IBlobStorageService"/> interface.
/// </summary>
internal class BlobStorageService : IBlobStorageService
{
    /// <summary>
    /// Holds the configuration settings for connecting to Azure Blob Storage.
    /// </summary>
    private readonly BlobStorageSettings _blobStorageSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlobStorageService"/> class.
    /// Retrieves blob storage settings from dependency injection.
    /// </summary>
    /// <param name="blobStorageSettingsOptions">Options pattern wrapper for <see cref="BlobStorageSettings"/>.</param>
    public BlobStorageService(IOptions<BlobStorageSettings> blobStorageSettingsOptions)
    {
        _blobStorageSettings = blobStorageSettingsOptions.Value;
    }

    /// <summary>
    /// Uploads a file stream to Azure Blob Storage and returns the URL of the uploaded file.
    /// </summary>
    /// <param name="data">The file stream to upload.</param>
    /// <param name="fileName">The name of the file in the storage container.</param>
    /// <returns>The URL of the uploaded blob.</returns>
    public async Task<string> UploadToBlobAsync(Stream data, string fileName)
    {
        // Create a client to interact with the Azure Blob Storage account.
        var blobServiceClient = new BlobServiceClient(_blobStorageSettings.ConnectionString);

        // Get a reference to the specific blob container where files will be stored.
        var containerClient = blobServiceClient.GetBlobContainerClient(_blobStorageSettings.LogosContainerName);

        // Get a reference to a blob (file) inside the container.
        var blobClient = containerClient.GetBlobClient(fileName);

        // Check if the blob already exists
        if (await blobClient.ExistsAsync())
        {
            throw new BlobAlreadyExistsException();
        }

        // Upload the file stream asynchronously to the blob.
        await blobClient.UploadAsync(data);

        // Retrieve and return the URL of the uploaded blob.
        return blobClient.Uri.ToString();
    }
}
