namespace Weezlabs.Storgage.UtilService
{
    using System;
    using System.Web.Configuration;

    /// <summary>
    /// Application settings.
    /// </summary>
    public class AppSettings : IAppSettings
    {
        /// <summary>
        /// Get application setting
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Value of setting</returns>
        public T GetSetting<T>(string key)
        {
            try
            {
                return (T) Convert.ChangeType(WebConfigurationManager.AppSettings[key], typeof (T));
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }
    }
}
