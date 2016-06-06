namespace Weezlabs.Storgage.FileUploadService
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Net.Http;

    /// <summary>
    /// Interface for file upload provider
    /// </summary>
    public interface IFileUploadProvider
    {
        /// <summary>
        /// Upload files to server
        /// </summary>
        /// <param name="path">Path to files uploading.</param>
        /// <param name="files">IEnumerable of HttpContent contains files.</param>
        /// <returns>Dictionary contained files with statuses.</returns>
        Dictionary<String, String> UploadFiles(String path, IEnumerable<HttpContent> files);

        /// <summary>
        /// Upload file to server
        /// </summary>
        /// <param name="path">Path to file uploading.</param>
        /// <param name="fileName">Name of file.</param>
        /// <param name="stream">Stream.</param>
        void UploadFile(String path, String fileName, Stream stream);
    }
}
