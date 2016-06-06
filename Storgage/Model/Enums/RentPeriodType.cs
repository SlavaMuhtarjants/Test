namespace Weezlabs.Storgage.Model.Enums
{
    /// <summary>
    /// Rent period type.
    /// </summary>
    public enum RentPeriodType
    {
        /// <summary>
        /// Rent period lesser than or equal to 3 months.
        /// </summary>
        LesserOrEqualThreeMonths,

        /// <summary>
        /// Rent period lesser than or equal to 6 months.
        /// </summary>
        LesserOrEqualSixMonths,

        /// <summary>
        /// Rent period lesser than or equal 1 year.
        /// </summary>
        LesserOrEqualYear,

        /// <summary>
        /// More than year.
        /// </summary>
        MoreYear
    }
}
