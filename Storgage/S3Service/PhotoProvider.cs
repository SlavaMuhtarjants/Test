namespace Weezlabs.Storgage.PhotoService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;

    using Model.Exceptions;
    using UtilService;
    using UtilService.Helpers;

    using Amazon;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Amazon.S3.Transfer;

    public class PhotoProvider : IPhotoProvider
    {
        private readonly IAppSettings appSettings;

        private IAmazonS3 AmazonClient
        {
            get
            {
                RegionEndpoint region =
                    RegionEndpoint.GetBySystemName(appSettings.GetSetting<String>("AWSRegionEndpoint"));
                IAmazonS3 client = AWSClientFactory.CreateAmazonS3Client(
                    appSettings.GetSetting<String>("AWSAccessKey"),
                    appSettings.GetSetting<String>("AWSSecretKey"),
                    region);
                return client;
            }
        }

        /// <summary>
        /// Default constructor of Photo provider
        /// </summary>
        /// <param name="appSettings">Application settings.</param>
        public PhotoProvider(IAppSettings appSettings)
        {
            Contract.Requires(appSettings != null);

            this.appSettings = appSettings;
        }

        /// <summary>
        /// Upload file to s3 bucket
        /// </summary>
        /// <param name="bucket">Name of bucket from webconfig.</param>
        /// <param name="pathToFile">Path to uploaded file.</param>
        /// <param name="fileName">Name of uploaded file.</param>
        /// <param name="fileStream">File stream.</param>
        /// <returns>Uploaded file name.</returns>
        public String UploadFile(String bucket, String pathToFile, String fileName, Stream fileStream)
        {
            try
            {
                var config = new TransferUtilityConfig();
                var upload = new TransferUtility(this.AmazonClient, config);

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucket,
                    CannedACL = S3CannedACL.PublicRead,
                    Key = pathToFile + fileName,
                    InputStream = fileStream
                };

                upload.Upload(uploadRequest);

                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
            }
            catch (AmazonS3Exception)
            {
                throw new FileLoadException(Resources.Messages.UploadFileError);
            }

            return fileName;
        }

        /// <summary>
        /// Upload file with thumbnails.
        /// </summary>
        /// <param name="bucket">S3 bucket.</param>
        /// <param name="originalPath">Path to original file.</param>
        /// <param name="thumbnailsPaths">Paths to thumbnails.</param>
        /// <param name="fileName">Name of uploaded file.</param>
        /// <param name="fileStream">File stream.</param>
        /// <returns>Name of ulpoaded file (without path).</returns>
        public String UploadFileWithThumbnails(String bucket, String originalPath, String thumbnailsPaths, String fileName, Stream fileStream)
        {
            String pathToOriginal = String.Format("{0}/", originalPath);
            String[] thumbnailsFolders = thumbnailsPaths.Split(';');

            foreach (string thumbnailsFolder in thumbnailsFolders)
            {
                var sideSize = Int32.Parse(thumbnailsFolder);
                var pathToThumbnail = String.Format("{0}/", thumbnailsFolder);

                // we should set position of stream to begin before use ResizeImage library
                fileStream.Seek(0, SeekOrigin.Begin);
                // we shouldn't dispose source stream before loading last file (original), 
                // so we use false for last argument of ResizeImage method
                this.UploadFile(bucket, pathToThumbnail, fileName,
                    fileStream.ResizeImage(sideSize, false));
            }

            fileStream.Seek(0, SeekOrigin.Begin);
            String originalFile = this.UploadFile(bucket, pathToOriginal, fileName, fileStream);

            return originalFile;
        }

        /// <summary>
        /// Delete files by identifiers.
        /// </summary>
        /// <param name="bucket">S3 bucket.</param>
        /// <param name="originalPath">Path to original file.</param>
        /// <param name="thumbnailsPaths">Paths to thumbnails.</param>
        /// <param name="fileNames">Ienumerable of files to delete.</param>
        public void DeleteFiles(String bucket, String originalPath, String thumbnailsPaths,
            IEnumerable<String> fileNames)
        {
            List<String> fileNamesList = fileNames.ToList();

            if (fileNamesList.Count == 0)
            {
                return;
            }

            String pathToOriginal = String.Format("{0}/", originalPath);
            String[] thumbnailsFolders = thumbnailsPaths.Split(';');

            DeleteObjectsRequest multiObjectDeleteRequest = new DeleteObjectsRequest {BucketName = bucket};

            foreach (String fileName in fileNamesList)
            {
                multiObjectDeleteRequest.AddKey(pathToOriginal + fileName, null);

                foreach (string thumbnailsFolder in thumbnailsFolders)
                {
                    var pathToThumbnail = String.Format("{0}/", thumbnailsFolder);
                    multiObjectDeleteRequest.AddKey(pathToThumbnail + fileName, null);
                }
            }

            try
            {
                DeleteObjectsResponse response = this.AmazonClient.DeleteObjects(multiObjectDeleteRequest);
            }
            catch (DeleteObjectsException)
            {
                throw new DeleteFileException(Resources.Messages.DeleteFileError);
            }
        }
    }
}
