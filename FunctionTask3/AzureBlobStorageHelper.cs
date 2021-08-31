using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ServerLessExample
{
    public class AzureBlobStorageHelper
    {
        private readonly string _connectionString;
        private readonly string _blobContainer;
        private readonly CloudBlobContainer _container;

        public AzureBlobStorageHelper(IConfiguration configuration)
        {
            _connectionString = configuration["AzureBlobStorageConnectionString"];
            _blobContainer = configuration["AzureBlobStorageContainer"];

            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            _container = blobClient.GetContainerReference(_blobContainer);
        }

        public async System.Threading.Tasks.Task<CloudBlobContainer> AddAsync(string filePath, string Message)
        {
            try
            {

                await _container.GetBlockBlobReference(filePath).UploadTextAsync(Message);
            }
            catch (Exception exc)
            {
                Trace.Write(exc.Message);

                return null;
            }
            return _container;
        }

    }
}
