﻿using System;
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
    public class TestAzureBlobStorage
    {
        [TestMethod]
        public void TestCreateBlobFromStream()
        {
            var blobService = new BlobService();
            blobService.CreateBlob("images-app", "text/test123.txt", new MemoryStream(Encoding.UTF8.GetBytes("whatever")));
        }

        [TestMethod]
        public void TestGetContainerDirectories()
        {
            var blobService = new BlobService();
            var directories = blobService.GetContainerDirectories("images-app");
        }

        [TestMethod]
        public void TestCreateContainer()
        {
            var blobService = new BlobService();
            var directories = blobService.TryCreateContainer("USERIDABC12345678");
        }

        [TestMethod]
        public void TestGetSingleBlobFile()
        {
            var blobService = new BlobService();
            var stream = blobService.GetBlob("images-app", "text/test123.txt");
            using (System.IO.FileStream output = new System.IO.FileStream(@"C:\Users\jacob\Desktop\blobfileasdf.txt", FileMode.Create))
            {
                stream.CopyTo(output);
            }
        }
    }
}
