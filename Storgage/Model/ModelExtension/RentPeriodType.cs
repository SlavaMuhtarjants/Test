namespace Weezlabs.Storgage.Model
{
    using System;    

    using Contracts;

    /// <summary>
    /// Rent period type extensions.
    /// </summary>
    public partial class RentPeriodType : IEnumConvertible<Enums.RentPeriodType>
    {
        /// <summary>
        /// Onverts model to enum.
        /// </summary>
        /// <returns>Enum.</returns>
        public Enums.RentPeriodType ToEnum()
        {
            return (Enums.RentPeriodType)Enum.Parse(typeof(Enums.RentPeriodType), Synonym);
        }
    }
}
