namespace Weezlabs.Storgage.SpaceService
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Weezlabs.Storgage.DataLayer.Spaces;
    using Weezlabs.Storgage.Model;

    public class ZipCodeProvider : IZipCodeProvider
    {
        public ZipCodeProvider(IZipRepository zipCodeRepository)
        {
            Contract.Requires(zipCodeRepository != null);

            this.zipCodeRepository = zipCodeRepository;
        }

        /// <summary>
        /// Returns zip code entity by zip code
        /// </summary>
        /// <param name="zipCode">zip code</param>
        /// <returns>Zip code entity</returns>
        public Zip Get(String zipCode)
        {
            return zipCodeRepository.GetAll().SingleOrDefault(z => z.ZipCode.Equals(zipCode));
        }

        private readonly IZipRepository zipCodeRepository;
    }
}