namespace Weezlabs.Storgage.MessagePublisher
{
    using System;

    /// <summary>
    /// Mobile endpoints provider interdace.
    /// </summary>    
    public interface IMobileEndpointProvider
    {
        /// <summary>
        /// Creates mobile endpoint.
        /// </summary>
        /// <param name="deviceToken">Device  token.</param>
        /// <param name="applicationEndpoint">Application endpoint (APNS, GCS))</param>
        /// <returns>Created mobile endpoint.</returns>
        MobileEndpointInfo CreateMobileEndpoint(String deviceToken, String applicationEndpoint);

        /// <summary>
        /// Removes mobile endpoint.
        /// </summary>
        /// <param name="mobileEndpointInfo">Mobile endpoint to remove.</param>
        void RemoveMobileEndpoint(MobileEndpointInfo mobileEndpointInfo);

        /// <summary>
        /// Checks that mobile endpoint is enabled.
        /// </summary>
        /// <param name="mobileEndpoint">Mobile endpoint.</param>
        /// <returns>True if mobile endpoint is enabled.</returns>
        Boolean IsMobileEndpointEnabled(String mobileEndpoint);
    }
}
