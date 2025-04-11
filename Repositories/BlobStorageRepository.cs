using Azure.Storage.Blobs;
using FamilySyncApi.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace FamilySyncApi.Repositories;

public class BlobStorageRepository<T> : IBlobStorageRepository<T>
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageRepository(IOptions<AzureBlobStorageSettings> options)
    {
        var settings = options.Value;
        _containerClient = new BlobContainerClient(settings.ConnectionString, settings.ContainerName);
    }

    public async Task<List<T>> GetAsync(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync())
            return new List<T>();

        try
        {
            var response = await blobClient.DownloadContentAsync();
            return JsonSerializer.Deserialize<List<T>>(response.Value.Content.ToString()) ?? new List<T>();
        }
        catch (Exception)
        {
            // Optional: Add logging here
            return new List<T>();
        }
    }

    public async Task SaveAsync(string blobName, List<T> data)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        var json = JsonSerializer.Serialize(data);
        await blobClient.UploadAsync(BinaryData.FromString(json), overwrite: true);
    }
}
