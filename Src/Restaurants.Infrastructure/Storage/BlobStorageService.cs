using Azure.Storage.Blobs;
using Azure.Storage.Sas;
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

    /// <summary>
    /// Generates a Shared Access Signature (SAS) URL for a given blob URL, 
    /// allowing temporary read access to the blob.
    /// </summary>
    /// <param name="blobUrl">The URL of the blob for which the SAS token is generated.</param>
    /// <returns>
    /// A SAS URL with a read permission token appended, or null if the input URL is invalid.
    /// </returns>
    public string? GetBlobSASUrl(string? blobUrl)
    {
        // Validate the input URL
        if (string.IsNullOrWhiteSpace(blobUrl)) return null;

        // Create a SAS builder for generating the token
        var sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = _blobStorageSettings.LogosContainerName, // Blob container name
            Resource = "b", // Specifies that the resource is a blob
            StartsOn = DateTimeOffset.UtcNow, // Start time (immediate activation)
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(30), // Expiry time (valid for 30 minutes)
            BlobName = GetBlobNameFromBlobUrl(blobUrl) // Extract blob name from URL
        };

        // Set read permissions for the generated SAS token
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        // Create a BlobServiceClient using the connection string
        var blobServiceClient = new BlobServiceClient(_blobStorageSettings.ConnectionString);

        // Generate the SAS token using storage account credentials
        var sasToken = sasBuilder.ToSasQueryParameters(
            new Azure.Storage.StorageSharedKeyCredential(
                blobServiceClient.AccountName,
                _blobStorageSettings.AccountKey
            )).ToString();

        // Append the SAS token to the blob URL and return it
        return $"{blobUrl}?{sasToken}";
    }


    private string GetBlobNameFromBlobUrl(string blobUrl)
    {
        var uri = new Uri(blobUrl);
        return uri.Segments.Last();
    }
}
