namespace Weezlabs.Storgage.DataTransferObjects.Stripe
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// Stripe exception throwed by restsharp
    /// </summary>
    public class StripeRestSharpException
    {
        /// <summary>
        /// Error
        /// </summary>
        [JsonProperty("error")]
        public ErrorMessage Error { get; set; }
    }

    public class ErrorMessage
    {
        /// <summary>
        /// Error message
        /// </summary>
        [JsonProperty("message")]
        public String Message { get; set; }
    }
}
