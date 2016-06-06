namespace Weezlabs.Storgage.Model
{
    using System;

    using Contracts;

    public partial class PhoneVerificationStatus : IEnumConvertible<Enums.PhoneVerificationStatus>
    {
        /// <summary>
        /// Converts model object to enum.
        /// </summary>
        /// <returns>Phone verification status enum.</returns>
        public Enums.PhoneVerificationStatus ToEnum()
        {
            return (Enums.PhoneVerificationStatus)Enum.Parse(typeof(Enums.PhoneVerificationStatus), Synonym);
        }
    }
}
