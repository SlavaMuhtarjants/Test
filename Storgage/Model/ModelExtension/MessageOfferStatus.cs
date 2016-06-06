namespace Weezlabs.Storgage.Model
{
    using System;

    using Contracts;

    /// <summary>
    /// Message status extension.
    /// </summary>
    public partial class MessageOfferStatus : IEnumConvertible<Enums.MessageOfferStatus>
    {
        /// <summary>
        /// Converts model object to enum.
        /// </summary>
        /// <returns>Message status enum.</returns>
        public Enums.MessageOfferStatus ToEnum()
        {
            return (Enums.MessageOfferStatus)Enum.Parse(typeof(Enums.MessageOfferStatus), Synonym);
        }
    }
}
