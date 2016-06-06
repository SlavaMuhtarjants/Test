using Stripe;

namespace Weezlabs.Storgage.DataTransferObjects.Stripe
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// Class for getting one more field from Stripe
    /// </summary>
    public class StripeBankAccount : global::Stripe.StripeBankAccount
    {
        [JsonProperty("account_holder_name")]
        public String AccountHolderName { get; set; }
    }
}
