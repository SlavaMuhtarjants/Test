namespace Weezlabs.Storgage.Model
{
    using System;

    using Contracts;


    /// <summary>
    /// Space type extension.
    /// </summary>
    public partial class SpaceType : IEnumConvertible<Enums.SpaceType>
    {
        /// <summary>
        /// Converts model object to enum.
        /// </summary>
        /// <returns>SpaceType enum.</returns>
        public Enums.SpaceType ToEnum()
        {
            return (Enums.SpaceType)Enum.Parse(typeof(Enums.SpaceType), Synonym);
        }
    }
}
