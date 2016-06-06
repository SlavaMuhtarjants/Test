using Stripe;

namespace Weezlabs.Storgage.DataTransferObjects.Stripe
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// Class for deserialize response with list of debit cards of managed account
    /// </summary>
    public class StripeDebitCardsListResponse
    {
        [JsonProperty("data")]
        public List<StripeCard> Data { get; set; }
    }
}
