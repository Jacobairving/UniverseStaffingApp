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

        public CloudBlockBlob GetBlob(string containerName, string blobName)
        {
            CloudBlobContainer container = _azureClient.GetContainerReference(NormalizeContainerName(containerName));
            return container.GetBlockBlobReference(blobName);
        }


        public MemoryStream GetBlobAsStream(string containerName, string blobName)
        {
            CloudBlockBlob blob = GetBlob(containerName, blobName);

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
            var blobContainer = _azureClient.GetContainerReference(NormalizeContainerName(container));

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);
            blockBlob.UploadFromStream(stream);
        }


        // NOTE: This method throws an exception of Type "StorageException" when an invalid container name is provided
        // The message describing the error is on a property of the exception named RequestInformation.HttpStatusMessage
        public CloudBlobContainer TryCreateUserContainer(string containerName)
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

            if (wasCreated)
            {
                container.SetPermissions(AccessPolicyManager.GetStandardUserBlobContainerPolicy());
            }

            // If it existed, return the existing reference, otherwise requery the container we created
            return container;
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
        
        public CloudBlobContainer GetBlobContainer(string containerName)
        {
            return _azureClient.GetContainerReference(NormalizeContainerName(containerName));
        }

        public string GetUserContainerSaSUri(string containerName)
        {
            return GetContainerSasUri(_azureClient.GetContainerReference(NormalizeContainerName(containerName)), "StandardUserPolicy");
        }

        public string GetContainerSasUri(CloudBlobContainer container, string storedPolicyName = null)
        {
            string sasContainerToken;

            // If no stored policy is specified, create a new access policy and define its constraints.
            if (storedPolicyName == null)
            {
                // Note that the SharedAccessBlobPolicy class is used both to define the parameters of an ad-hoc SAS, and
                // to construct a shared access policy that is saved to the container's shared access policies.
                SharedAccessBlobPolicy adHocPolicy = new SharedAccessBlobPolicy()
                {
                    // When the start time for the SAS is omitted, the start time is assumed to be the time when the storage service receives the request.
                    // Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                    Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List
                };

                // Generate the shared access signature on the container, setting the constraints directly on the signature.
                sasContainerToken = container.GetSharedAccessSignature(adHocPolicy, null);
            }
            else
            {
                // Generate the shared access signature on the container. In this case, all of the constraints for the
                // shared access signature are specified on the stored access policy, which is provided by name.
                // It is also possible to specify some constraints on an ad-hoc SAS and others on the stored access policy.
                sasContainerToken = container.GetSharedAccessSignature(null, storedPolicyName);
            }

            // Return the URI string for the container, including the SAS token.
            return container.Uri + sasContainerToken + "&comp=list&restype=container";
        }

        public string GetBlobSasUri(string containerName, string blobName)
        {
            //Get a reference to a blob within the container.
            CloudBlockBlob blob = GetBlob(containerName, blobName);

            // Get the standard user access policy to apply to this blob request
            SharedAccessBlobPolicy blobAccessPolicy = AccessPolicyManager.GetStandardUserBlobPolicy();

            //Generate the shared access signature on the blob, setting the constraints directly on the signature.
            string sasBlobToken = blob.GetSharedAccessSignature(blobAccessPolicy);

            //Return the URI string for the container, including the SAS token.
            return blob.Uri + sasBlobToken;
        }

    }
}
