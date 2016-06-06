namespace Weezlabs.Storgage.MessagePublisher
{
    using System;

    public sealed class MobileEndpointInfo
    {
        /// <summary>
        /// Mobile endpoint.
        /// </summary>
        public String MobileEndpoint { get; set; }

        /// <summary>
        /// Subscription endpoint.
        /// </summary>
        public String SubscriptionEndpoint { get; set; }

        /// <summary>
        /// Topic endpooint.
        /// </summary>
        public String TopicEndpoint { get; set; }
    }
}
