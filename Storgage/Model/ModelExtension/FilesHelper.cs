using System.Diagnostics.Contracts;

namespace Weezlabs.Storgage.Model.ModelExtension
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;

    /// <summary>
    /// Files helper.
    /// </summary>
    public static class FilesHelper
    {
        /// <summary>
        /// Checking files to maxLength
        /// </summary>
        /// <param name="files">Ienumerable of uploading files.</param>
        /// <param name="maxFileLength">Max length of uploading file.</param>
        public static void CheckFilesLength(this IEnumerable<HttpContent> files, Int32 maxFileLength)
        {
            foreach (HttpContent content in files)
            {
                Int64? curFileLength = content.Headers.ContentLength;
                if (content.Headers.ContentLength > maxFileLength)
                {
                    throw new BadImageFormatException(String.Format(Resources.Messages.MaximumImageLength,
                        maxFileLength,
                        curFileLength));
                }
            }
        }

        /// <summary>
        /// Generate new name of file for file
        /// </summary>
        /// <param name="file">HttpContent contains file.</param>
        /// <returns>New name for file (based on Guid) with extension from request header.</returns>
        public static String GenerateFileNameWithExt(this HttpContent file)
        {
            Contract.Requires(file != null);

            String fileName = file.Headers.ContentDisposition.FileName.Replace("\"", String.Empty);

            return GenerateFileNameWithExt(fileName);
        }

        /// <summary>
        /// Generate new name of file by original name
        /// </summary>
        /// <param name="fileName">Original file name.</param>
        /// <returns></returns>
        public static String GenerateFileNameWithExt(String fileName)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(fileName));

            String fileExt = Path.GetExtension(fileName);
            String newFileName = Guid.NewGuid() + fileExt;
            
            return newFileName;
        }
    }
}
