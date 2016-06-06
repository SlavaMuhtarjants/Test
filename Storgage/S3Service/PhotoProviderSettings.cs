namespace Weezlabs.Storgage.PhotoService
{
    using System;
    using System.Diagnostics.Contracts;

    using UtilService;

    /// <summary>
    /// Photo provider settings got from webconfig.
    /// </summary>
    public static class PhotoProviderSettings
    {
        #region S3 user bucket settings 

        /// <summary>
        /// Get S3 user avatars bucket
        /// </summary>
        /// <param name="appSettings">AppSettings</param>
        /// <returns>S3 bucket name for user avatars.</returns>
        public static String GetS3UsersBucket(this IAppSettings appSettings)
        {
            Contract.Requires(appSettings != null);

            return appSettings.GetSetting<String>("s3usersBucket");
        }

        /// <summary>
        /// Get S3 user avatars bucket original folder
        /// </summary>
        /// <param name="appSettings">AppSettings</param>
        /// <returns>Name of original folder on S3 for user avatars.</returns>
        public static String GetS3UsersOriginal(this IAppSettings appSettings)
        {
            Contract.Requires(appSettings != null);

            return appSettings.GetSetting<String>("s3usersOriginal");
        }

        /// <summary>
        /// Get S3 user avatars bucket thumbnails folders
        /// </summary>
        /// <param name="appSettings">AppSettings</param>
        /// <returns>String with separated folders for thumbnails of user avatars on S3.</returns>
        public static String GetS3UsersThumbnails(this IAppSettings appSettings)
        {
            Contract.Requires(appSettings != null);

            return appSettings.GetSetting<String>("s3usersThumbnails");
        }

        #endregion

        #region S3 storgage photo bucket settings 

        /// <summary>
        /// Get S3 storgage photo bucket
        /// </summary>
        /// <param name="appSettings">AppSettings</param>
        /// <returns>S3 bucket name for storgage photos.</returns>
        public static String GetS3SpacesBucket(this IAppSettings appSettings)
        {
            Contract.Requires(appSettings != null);

            return appSettings.GetSetting<String>("s3spacesBucket");
        }

        /// <summary>
        /// Get S3 storgage photo bucket original folder
        /// </summary>
        /// <param name="appSettings">AppSettings</param>
        /// <returns>Name of original folder on S3 for storgage photos.</returns>
        public static String GetS3SpacesOriginal(this IAppSettings appSettings)
        {
            Contract.Requires(appSettings != null);

            return appSettings.GetSetting<String>("s3spacesOriginal");
        }

        /// <summary>
        /// Get S3 storgage photo bucket thumbnails folders
        /// </summary>
        /// <param name="appSettings">AppSettings</param>
        /// <returns>String with separated folders for thumbnails of storgage photos on S3.</returns>
        public static String GetS3SpacesThumbnails(this IAppSettings appSettings)
        {
            Contract.Requires(appSettings != null);

            return appSettings.GetSetting<String>("s3spacesThumbnails");
        }

        #endregion
    }
}
