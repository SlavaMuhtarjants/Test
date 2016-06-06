namespace Weezlabs.Storgage.Model
{
    using System;

    using Contracts;

    /// <summary>
    /// Extension to PaymentPointType.
    /// </summary>
    public partial class PaymentPointType : IEnumConvertible<Enums.PaymentPointType>
    {
        /// <summary>
        /// Converts PaymentPointType model object to enum.
        /// </summary>
        /// <returns>PaymentPointType enum.</returns>
        public Enums.PaymentPointType ToEnum()
        {
            return (Enums.PaymentPointType)Enum.Parse(typeof(Enums.PaymentPointType), Synonym);
        }
    }
}
