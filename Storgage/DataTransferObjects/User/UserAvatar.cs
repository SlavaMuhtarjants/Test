namespace Weezlabs.Storgage.DataTransferObjects.User
{
    using System;
    using System.Web.Configuration;

    /// <summary>
    /// Contains information about user avatar.
    /// </summary>
    public class UserAvatar
    {
        /// <summary>
        /// Link to S3 bucket with user avatars.
        /// </summary>
        public String Link { get; set; }

        /// <summary>
        /// Name of avatar file.
        /// </summary>
        public String FileName { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UserAvatar()
        {
        }

        /// <summary>
        /// Creates UserAvatar from model.
        /// </summary>
        /// <param name="fileName">Name of avatar file.</param>
        public UserAvatar(String fileName)
        {
            Link = !String.IsNullOrEmpty(fileName)
                ? WebConfigurationManager.AppSettings["s3usersImagesUrl"]
                : string.Empty;
            FileName = !String.IsNullOrEmpty(fileName) ? fileName : string.Empty;
        }
    }
}
