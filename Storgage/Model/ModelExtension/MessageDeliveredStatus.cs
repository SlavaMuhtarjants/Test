namespace Weezlabs.Storgage.Model
{
    using System;
    using Contracts;

    /// <summary>
    /// Message status extension.
    /// </summary>
    public partial class MessageDeliveredStatus : IEnumConvertible<Enums.MessageDeliveredStatus>
    {
        /// <summary>
        /// Converts model object to enum.
        /// </summary>
        /// <returns>Message status enum.</returns>
        public Enums.MessageDeliveredStatus ToEnum()
        {
            return (Enums.MessageDeliveredStatus)Enum.Parse(typeof(Enums.MessageDeliveredStatus), Synonym);
        }
    }
}
