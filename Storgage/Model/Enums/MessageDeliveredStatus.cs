namespace Weezlabs.Storgage.Model.Enums
{
    /// <summary>
    /// Message statuses
    /// </summary>
    public enum MessageDeliveredStatus
    {
        /// <summary>
        /// Message was read
        /// </summary>
        WasRead,

        /// <summary>
        /// Message was sent to user
        /// </summary>
        WasSent,

        /// <summary>
        /// Message has not been sent to user yet
        /// </summary>
        WasNotSent
    }
}