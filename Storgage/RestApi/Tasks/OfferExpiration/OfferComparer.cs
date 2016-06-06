namespace Weezlabs.Storgage.RestApi.Tasks.OfferExpiration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Offer comparer.
    /// </summary>
    internal class OfferComparer : IEqualityComparer<ExpiredOfferInfo>
    {
        /// <summary>
        /// Equal two expired offers info.
        /// </summary>
        /// <param name="x">Comparable object.</param>
        /// <param name="y">Compared obkject.</param>
        /// <returns>Returns true if two objects are equal.</returns>
        public Boolean Equals(ExpiredOfferInfo x, ExpiredOfferInfo y)
        {
            return x.UserID == y.UserID && x.ChatID == y.ChatID;
        }

        /// <summary>
        /// Returns get hash code.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <returns>Returns hash code.</returns>
        public Int32 GetHashCode(ExpiredOfferInfo obj)
        {
            return obj.OfferID.GetHashCode();
        }
    }
}