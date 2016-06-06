namespace Weezlabs.Storgage.AbuseService
{
    using System;
    using System.Diagnostics.Contracts;

    using UtilService;

    public static class AbuseProviderSettings
    {
        /// <summary>
        /// Returns Bucket for Contact Us
        /// </summary>
        /// <param name="appSettings">AppSettings</param>
        /// <returns>Name of original folder on S3 for contact us</returns>
        public static String GetS3ContactUsBucket(this IAppSettings appSettings)
        {
            Contract.Requires(appSettings != null);

            return appSettings.GetSetting<String>("s3ContactUsBucket");
        }

        /// <summary>
        /// Get S3 contact us file bucket original folder
        /// </summary>
        /// <param name="appSettings">AppSettings</param>
        /// <returns>Name of original folder on S3 for contact us.</returns>
        public static String GetS3ContactUsOriginal(this IAppSettings appSettings)
        {
            Contract.Requires(appSettings != null);

            return appSettings.GetSetting<String>("s3ContactUsOriginal");
        }
        
        /// <summary>
        /// Get S3 contact us file URL
        /// </summary>
        /// <param name="appSettings">AppSettings</param>
        /// <returns>Name of original folder on S3 for contact us.</returns>
        public static String GetS3ContactUsFileUrl(this IAppSettings appSettings)
        {
            Contract.Requires(appSettings != null);

            return appSettings.GetSetting<String>("s3contactUsFileUrl");
        }

           
    }
}
