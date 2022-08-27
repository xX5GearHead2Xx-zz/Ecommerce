using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Storage.Blob;

namespace Ecommerce.Models
{
    public class AzureStorage
    {
        private static string ConnectionString;
        private static string ContainerName;

        public AzureStorage()
        {
            ConnectionString = Startup.StaticConfiguration["AzureStorage_ProductImages:ConnectionString"];
            ContainerName = Startup.StaticConfiguration["AzureStorage_ProductImages:Container"];
        }

        public bool Upload(string FileKey, byte[] Data)
        {
            try
            {
                var cloudStorageAccount = Microsoft.Azure.Storage.CloudStorageAccount.Parse(ConnectionString);
                var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                var cloudBlobContainer = cloudBlobClient.GetContainerReference(ContainerName);
                var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(FileKey);
                if (!cloudBlockBlob.Exists())
                {
                    using (Stream stream = new MemoryStream(Data))
                    {
                        cloudBlockBlob.UploadFromStream(stream);
                    }
                    return true;
                }
                else
                {
                    if (Delete(FileKey))
                    {
                        Upload(FileKey, Data);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > AzureStorage > UploadImage " + Ex.Message);
            }
        }

        public bool Delete(string FileKey)
        {
            try
            {
                var cloudStorageAccount = Microsoft.Azure.Storage.CloudStorageAccount.Parse(ConnectionString);
                var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                var cloudBlobContainer = cloudBlobClient.GetContainerReference(ContainerName);
                var blob = cloudBlobContainer.GetBlockBlobReference(FileKey);
                blob.DeleteIfExists();
                return true;
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > AzureStorage >  DeleteImage" + Ex.Message);
            }
        }

        public byte[] Download(string FileKey)
        {
            try
            {
                var cloudStorageAccount = Microsoft.Azure.Storage.CloudStorageAccount.Parse(ConnectionString);
                var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                var cloudBlobContainer = cloudBlobClient.GetContainerReference(ContainerName);
                var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(FileKey);
                if (cloudBlockBlob.Exists())
                {
                    byte[] data = new byte[cloudBlockBlob.Properties.Length];
                    cloudBlockBlob.DownloadToByteArray(data,0);
                    return data;
                }
                return null;
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > AzureStorage > UploadImage " + Ex.Message);
            }
        }
    }
}
