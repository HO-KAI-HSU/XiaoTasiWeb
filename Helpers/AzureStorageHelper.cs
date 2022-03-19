using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using xiaotasi.Data;

namespace xiaotasi.Helpers
{
    public class AzureStorageHelper
    {

        /*Smtp參數設定*/
        private string _accountName = "";
        private string _accountKey = "";
        private readonly IConfiguration _config;


        public AzureStorageHelper(IConfiguration config)
        {
            _config = config;
            _accountName = _config.GetValue<string>("AzureStorage:ConnectionStrings:AccountName");
            _accountKey = _config.GetValue<string>("AzureStorage:ConnectionStrings:AccountKey");
        }

        public async Task<string> UploadFileToStorage(Stream fileStream, string containerName, string groupName, string fileName)
        {
            // Create a URI to the blob
            Uri blobUri = new Uri("https://" +
                                _accountName +
                                ".blob.core.windows.net/" +
                                containerName + "/" +
                                groupName + "/" +
                                fileName);

            // Create StorageSharedKeyCredentials object by reading
            // the values from the configuration (appsettings.json)
            StorageSharedKeyCredential storageCredentials =
                new StorageSharedKeyCredential(_accountName, _accountKey);

            // Create the blob client.
            BlobClient blobClient = new BlobClient(blobUri, storageCredentials);



            var fileBytes = ReadFully(fileStream);
            var data = new BinaryData(fileBytes);

            // Upload the file
            await blobClient.UploadAsync(data);

            return blobUri.ToString();
        }

        private static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
