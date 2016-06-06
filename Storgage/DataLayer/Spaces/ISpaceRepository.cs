namespace Weezlabs.Storgage.DataLayer.Spaces
{
    using System;

    using Model;

    /// <summary>
    /// Repository interface for Spaces.
    /// </summary>
    public interface ISpaceRepository  : IRepository<Space, Guid>
    {
        /// <summary>
        /// Methods returns forecasted rate
        /// </summary>
        /// <param name="SizeTypeId">Space Type Id</param>
        /// <param name="zipCode">Zip code</param>
        /// <returns>forecasted rate</returns>
        Decimal GetForecastedRate(Guid sizeTypeId, String zipCode);
    }
}
