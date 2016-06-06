namespace Weezlabs.Storgage.Model
{
    using System;

    using Contracts;

    /// <summary>
    /// Extension to size type.
    /// </summary>
    public partial class SizeType : IEnumConvertible<Enums.SizeType>
    {
        /// <summary>
        /// Converts size type model object to enum.
        /// </summary>
        /// <returns>SizeType enum.</returns>
        public Enums.SizeType ToEnum()
        {
            return (Enums.SizeType) Enum.Parse(typeof(Enums.SizeType), Synonym);
        }
    }
}
