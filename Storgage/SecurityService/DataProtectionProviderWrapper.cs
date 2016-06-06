namespace Weezlabs.Storgage.SecurityService
{
    using Microsoft.Owin.Security.DataProtection;

    /// <summary>
    /// Wrapper for Data protection provider
    /// for fixed error: http://tech.trailmax.info/2014/06/asp-net-identity-and-cryptographicexception-when-running-your-site-on-microsoft-azure-web-sites/ 
    /// </summary>
    public class DataProtectionProviderWrapper
    {
        /// <summary>
        /// Data protection provider
        /// </summary>
        public static IDataProtectionProvider DataProtectionProvider { get; set; }
    }
}
