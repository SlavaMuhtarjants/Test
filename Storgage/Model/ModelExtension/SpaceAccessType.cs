namespace Weezlabs.Storgage.Model
{
    using System;

    using Contracts;

    /// <summary>
    /// Space access type extension.
    /// </summary>
    public partial class SpaceAccessType : IEnumConvertible<Enums.SpaceAccessType>
    {
        /// <summary>
        /// Converts model object to enum.
        /// </summary>
        /// <returns>SpaceAccessType enum.</returns>
        public Enums.SpaceAccessType ToEnum()
        {
            return (Enums.SpaceAccessType)Enum.Parse(typeof(Enums.SpaceAccessType), Synonym);
        }
    }
}
