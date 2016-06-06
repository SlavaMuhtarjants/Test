namespace Weezlabs.Storgage.UserNotifier.Notifications
{
    /// <summary>
    /// Event types.
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// Tenant started a new chat.
        /// </summary>
        NewChatStarted,

        /// <summary>
        /// New message or offer was posted.
        /// </summary>
        NewMessagePosted,

        /// <summary>
        /// Spaces matching user filter have appeared
        /// </summary>
        NewRelevantSpaces,

        /// <summary>
        /// Offer was approved.
        /// </summary>
        OfferWasApproved,

        /// <summary>
        /// Offer was expired.
        /// </summary>
        OfferWasExpired,

        /// <summary>
        /// Offer was rejected.
        /// </summary>
        OfferWasRejected,

        /// <summary>
        /// Offer was stopped
        /// </summary>
        OfferWasStopped,
    }
}
