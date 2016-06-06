namespace Weezlabs.Storgage.FileUploadService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;

    /// <summary>
    /// File upload provider.
    /// </summary>
    public class FileUploadProvider : IFileUploadProvider
    {
        /// <summary>
        /// Upload files to server
        /// </summary>
        /// <param name="path">Path to files uploading.</param>
        /// <param name="files">IEnumerable of HttpContent contains files.</param>
        /// <returns>Dictionary contained files with statuses.</returns>
        public Dictionary<String, String> UploadFiles(String path, IEnumerable<HttpContent> files)
        {
            var result = new Dictionary<String, String>();

            foreach (HttpContent content in files)
            {
                Stream stream = content.ReadAsStreamAsync().Result;
                String fileName = content.Headers.ContentDisposition.FileName.Replace("\"", String.Empty);

                try
                {
                    UploadFile(path, fileName, stream);
                    result.Add(fileName, "Ok");
                }
                catch (Exception ex)
                {
                    result.Add(fileName, ex.Message);
                }
            }

            return result;
        }

        /// <summary>
        /// Upload file to server
        /// </summary>
        /// <param name="path">Path to file uploading with final /.</param>
        /// <param name="fileName">Name of file.</param>
        /// <param name="stream">Stream.</param>
        public void UploadFile(String path, String fileName, Stream stream)
        {
            var fullFileName = Path.Combine(path, fileName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (File.Exists(fullFileName))
            {
                throw new FileLoadException(String.Format(Resources.Messages.FileExists, fileName));
            }

            using (FileStream fileStream = File.Create(fullFileName))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }
        }
    }
}
