namespace Weezlabs.Storgage.UtilService
{
    using System;

    /// <summary>
    /// Interface for getting app settings
    /// </summary>
    public interface IAppSettings
    {
        /// <summary>
        /// Get application setting
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Value of setting</returns>
        T GetSetting<T>(String key);
    }
}
