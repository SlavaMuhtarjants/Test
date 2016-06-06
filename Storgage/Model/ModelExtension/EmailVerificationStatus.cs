namespace Weezlabs.Storgage.Model
{
    using System;

    using Contracts;

    public partial class EmailVerificationStatus : IEnumConvertible<Enums.EmailVerificationStatus>
    {
        /// <summary>
        /// Converts model object to enum.
        /// </summary>
        /// <returns>Email verification status enum.</returns>
        public Enums.EmailVerificationStatus ToEnum()
        {
            return (Enums.EmailVerificationStatus) Enum.Parse(typeof (Enums.EmailVerificationStatus), Synonym);
        }
    }
}
