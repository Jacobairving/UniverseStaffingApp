using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniverseStaffingApp.Data;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types

namespace UniverseStaffingApp.Data.Test
{
    [TestClass]
    public class BlobStorageTests
    {
        [TestMethod]
        public void CreateBlobFromStream()
        {
            var blobService = new BlobService();
            blobService.CreateBlob("USERID11111", "test123.txt", new MemoryStream(Encoding.UTF8.GetBytes("whatever")));
        }

        [TestMethod]
        public void GetContainerDirectories()
        {
            var blobService = new BlobService();
            var directories = blobService.GetContainerDirectories("images-app");
        }

        [TestMethod]
        public void CreateContainer()
        {
            var blobService = new BlobService();
            var container = blobService.TryCreateUserContainer("USERID11111");
        }

        [TestMethod]
        public void GetContainer()
        {
            var blobService = new BlobService();
            var container = blobService.GetBlobContainer("USERID11111");
        }

        [TestMethod]
        public void GetContainerPermissions()
        {
            var blobService = new BlobService();
            var container = blobService.GetBlobContainer("USERID11111");
            var permissions = container.GetPermissions();
        }

        [TestMethod]
        public void GetContainerSaSToken()
        {
            var blobService = new BlobService();
            var SaSUri = blobService.GetUserContainerSaSUri("USERID11111");

        }

        [TestMethod]
        public void GetBlobSasToken()
        {
            var blobService = new BlobService();
            var SaSUri = blobService.GetBlobSasUri("USERID11111", "test123.txt");

        }

        [TestMethod]
        public void GetSingleBlobFile()
        {
            var blobService = new BlobService();
            var stream = blobService.GetBlobAsStream("images-app", "text/test123.txt");
            using (System.IO.FileStream output = new System.IO.FileStream(@"C:\Users\jacob\Desktop\blobfileasdf.txt", FileMode.Create))
            {
                stream.CopyTo(output);
            }
        }
    }
}
