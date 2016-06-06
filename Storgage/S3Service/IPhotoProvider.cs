namespace Weezlabs.Storgage.PhotoService
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public interface IPhotoProvider
    {
        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="bucket">Name of bucket from webconfig.</param>
        /// <param name="pathToFile">Path to uploaded file.</param>
        /// <param name="fileName">Name of uploaded file.</param>
        /// <param name="fileStream">File stream.</param>
        /// <returns>Uploaded file name.</returns>
        String UploadFile(String bucket, String pathToFile, String fileName, Stream fileStream);

        /// <summary>
        /// Upload file with thumbnails.
        /// </summary>
        /// <param name="bucket">S3 bucket.</param>
        /// <param name="originalPath">Path to original file.</param>
        /// <param name="thumbnailsPaths">Paths to thumbnails.</param>
        /// <param name="fileName">Name of uploaded file.</param>
        /// <param name="fileStream">File stream.</param>
        /// <returns>Name of ulpoaded file (without path).</returns>
        String UploadFileWithThumbnails(String bucket, String originalPath, String thumbnailsPaths, String fileName, Stream fileStream);

        /// <summary>
        /// Delete files by identifiers.
        /// </summary>
        /// <param name="bucket">S3 bucket.</param>
        /// <param name="originalPath">Path to original file.</param>
        /// <param name="thumbnailsPaths">Paths to thumbnails.</param>
        /// <param name="fileNames">Ienumerable of files to delete.</param>
        void DeleteFiles(String bucket, String originalPath, String thumbnailsPaths,
            IEnumerable<String> fileNames);
    }
}
