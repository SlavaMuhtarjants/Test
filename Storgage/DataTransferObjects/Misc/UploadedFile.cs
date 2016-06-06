namespace Weezlabs.Storgage.DataTransferObjects.Misc
{
    using System;

    /// <summary>
    /// Contains information about Uploaded File.
    /// </summary>
    public class UploadedFile
    {
        /// <summary>
        /// Link to S3 bucket with uploaded.
        /// </summary>
        public String Link { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        public String FileName { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UploadedFile(String fileName, String link)
        {
            Link = link;
            FileName = fileName;
        }

    }
}
