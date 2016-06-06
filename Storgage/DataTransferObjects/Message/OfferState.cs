namespace Weezlabs.Storgage.DataTransferObjects.Message
{
    using System;
    using System.Diagnostics.Contracts;
    using System.ComponentModel.DataAnnotations;

    using Model;    
    using User;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;    


    /// <summary>
    /// Contains information about offer state.
    /// </summary>
    public class OfferState
    {
        /// <summary>
        /// Offer state identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Who changed offer state.
        /// </summary>
        public UserInfo ChangedBy { get; set; }

        /// <summary>
        /// When offer state was changed.
        /// </summary>
        public DateTimeOffset ChangedAt { get; set; }

        /// <summary>
        /// Offer status.
        /// </summary>
        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public Model.Enums.MessageOfferStatus Status { get; set; }


        /// <summary>
        /// Default constructor.
        /// </summary>
        public OfferState()
        { }

        /// <summary>
        /// Creates DTO from model.
        /// </summary>
        /// <param name="state">Model object.</param>
        public OfferState(MessageOfferHistory state)
        {
            Contract.Requires(state != null);

            Id = state.Id;
            ChangedBy = state.MessageOffer.CanDisplayUserContactInfo()
                ? new UserFullInfo(state.User)
                : new UserInfo(state.User);
            ChangedAt = state.ChangedStatusDate;
            Status = state.MessageOfferStatus.ToEnum();
        }
    }
}
