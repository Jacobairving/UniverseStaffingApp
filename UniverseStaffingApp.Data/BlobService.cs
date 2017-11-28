using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UniverseStaffingApp.Data
{
    public class BlobService
    {
        private CloudBlobClient _azureClient;

        public BlobService()
        {
            _azureClient = CreateBlobClient();
        }

        public MemoryStream GetBlob(string containerName, string blobName)
        {
            CloudBlobContainer container = _azureClient.GetContainerReference(containerName);
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            var stream = new MemoryStream();

            blob.DownloadToStream(stream);
            stream.Position = 0;

            return stream;
        }
        
        public List<CloudBlockBlob> GetBlobs(string container)
        {
            return GetBlobs(_azureClient.GetContainerReference(container));
        }

        public List<CloudBlockBlob> GetBlobs(CloudBlobContainer container)
        {
            return container.ListBlobs(null, true) as List<CloudBlockBlob>;
        }

       
        public List<CloudBlobDirectory> GetContainerDirectories(string container)
        {
            return GetContainerDirectories(_azureClient.GetContainerReference(container));
        }

        public List<CloudBlobDirectory> GetContainerDirectories(CloudBlobContainer container)
        {
            var directories = new List<CloudBlobDirectory>();

            // Loop over items within the container and output the length and URI.
            foreach (IListBlobItem item in container.ListBlobs(null, false))
            {
                if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    directories.Add((CloudBlobDirectory)item);
                }
            }

            return directories;
        }

        public void CreateBlob(string container, string blobName, Stream stream)
        {
            // Retrieve reference to a previously created container.
            var blobContainer = _azureClient.GetContainerReference(container);

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);
            blockBlob.UploadFromStream(stream);
        }


        // NOTE: This method throws an exception of Type "StorageException" when an invalid container name is provided
        // The message describing the error is on a property of the exception named RequestInformation.HttpStatusMessage
        public CloudBlobContainer TryCreateContainer(string containerName)
        {
            // Retrieve a reference to a container.
            CloudBlobContainer container = _azureClient.GetContainerReference(NormalizeContainerName(containerName));
            var wasCreated = false;

            try
            {
                wasCreated = container.CreateIfNotExists();
            }
            catch (StorageException ex)
            {
                throw ex;
            }
            
            // If it existed, return the existing reference, otherwise requery the container we created
            return wasCreated ? container : _azureClient.GetContainerReference(containerName);
        }

        private CloudBlobClient CreateBlobClient()
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("BlobStorageConnectionString"));

            // Create the blob client.
            return storageAccount.CreateCloudBlobClient();
        }

        private string NormalizeContainerName(string name)
        {
            string normalizedName = "";
            normalizedName = name.ToLower();
            return normalizedName;
        }
    }
}
