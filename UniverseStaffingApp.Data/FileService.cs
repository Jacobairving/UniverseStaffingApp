using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using Microsoft.Azure;
using System.IO;

namespace UniverseStaffingApp.Data
{
    public class FileService
    {
        private BlobService _blobService;

        public FileService()
        {
            _blobService = new BlobService();
        }

        public MemoryStream GetUserFile(string fileUri)
        {
            var userId = "ABC123456";
            var container = userId;
            return _blobService.GetBlobAsStream(container, fileUri);
        }
    }
}
