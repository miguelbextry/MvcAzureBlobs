using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using MvcAzureBlobs.Models;

namespace MvcAzureBlobs.Services
{
    public class ServiceStorageBlobs
    {
        private BlobServiceClient client;

        public ServiceStorageBlobs(BlobServiceClient client)
        {
            this.client = client;
        }
     

        public async Task UploadBlobAsync(string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            if (await blobClient.ExistsAsync())
            {
                await blobClient.DeleteAsync();
            }

            await containerClient.UploadBlobAsync(blobName, stream);
        }


        public async Task MoveBlobs(string contenedorOrigen, string contenedorDestino, string nombreimagen)
        {
            BlobContainerClient blobContainerSource = client.GetBlobContainerClient(contenedorOrigen);
            BlobClient sourceBlobClient = blobContainerSource.GetBlobClient(nombreimagen);
            BlobContainerClient blobContainerDestination = client.GetBlobContainerClient(contenedorDestino);
            BlobClient destinationBlobClient = blobContainerDestination.GetBlobClient(nombreimagen);

            CopyFromUriOperation operation = destinationBlobClient.StartCopyFromUri(sourceBlobClient.Uri);
            await operation.WaitForCompletionAsync();

            if (operation.HasCompleted)
            {
                sourceBlobClient.DeleteIfExists();

            }
        }
        public async Task<List<ModeloAzureBlobs>> GetBlobsAsync(string containerName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            List<ModeloAzureBlobs> blobmodels = new List<ModeloAzureBlobs>();
            var response = await containerClient.GetPropertiesAsync();
            var properties = response.Value;

            await foreach (BlobItem item in containerClient.GetBlobsAsync())
            {
                BlobClient blobClient = containerClient.GetBlobClient(item.Name);

                string imageUri = null;
                if (properties.PublicAccess == PublicAccessType.None)
                {
                    imageUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddSeconds(3600)).ToString();
                }
                else
                {
                    imageUri = blobClient.Uri.AbsoluteUri.ToString();
                }

                ModeloAzureBlobs model = new ModeloAzureBlobs
                {
                    Nombre = blobClient.Name,
                    Url = imageUri,
                };
                blobmodels.Add(model);
            }
            return blobmodels;
        }
    }
}
