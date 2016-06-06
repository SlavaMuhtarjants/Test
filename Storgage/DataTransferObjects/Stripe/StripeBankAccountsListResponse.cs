namespace Weezlabs.Storgage.DataTransferObjects.Stripe
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// Class for deserialize response with list of bank accounts
    /// </summary>
    public class StripeBankAccountsListResponse
    {
        [JsonProperty("data")]
        public List<StripeBankAccount> Data { get; set; }
    }
}
