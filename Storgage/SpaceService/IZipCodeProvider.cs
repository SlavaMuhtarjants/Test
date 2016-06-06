namespace Weezlabs.Storgage.SpaceService
{
    using System;

    using Weezlabs.Storgage.Model;

    public interface IZipCodeProvider
    {
        /// <summary>
        /// Returns zip code entity by zip code
        /// </summary>
        /// <param name="zipCode">zip code</param>
        /// <returns>Zip code entity</returns>
        Zip Get(String zipCode);
    }
}