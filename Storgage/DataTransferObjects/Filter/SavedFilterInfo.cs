namespace Weezlabs.Storgage.DataTransferObjects.Filter
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Model;
    using Resources;
    using Space;

    /// <summary>
    /// Contains information about user space filters
    /// </summary>
    public class SavedFilterInfo : FilterBase
    {
        /// <summary>
        /// Zip code
        /// </summary>
        public String ZipCode { get; set; }

        /// <summary>
        /// Location (city, state)
        /// </summary>
        public String Location { get; set; }

        /// <summary>
        /// Date a rent can start from
        /// </summary>
        public DateTimeOffset RentStartDate { get; set; }

        /// <summary>
        /// Constructor initialising the instance by database object
        /// </summary>
        /// <param name="filter">object of Filter table</param>
        public SavedFilterInfo(Filter filter)
        {
            Contract.Requires(filter != null);

            Id = filter.Id;
            MinRate = filter.MinPrice;
            MaxRate = filter.MaxPrice;
            RentStartDate = filter.RentStartDate;
            ZipCode = filter.ZipCodeId.HasValue ? filter.Zip.ZipCode : null;
            Location = filter.Location;

            AccessTypes = filter.FilterRootDictionaries.Select(frd => frd.RootDictionary.SpaceAccessType).
                Where(sat => sat != null).Select(sat => sat.ToEnum()).ToArray();
            SpaceTypes = filter.FilterRootDictionaries.Select(frd => frd.RootDictionary.SpaceType).
                Where(st => st != null).Select(st => st.ToEnum()).ToArray();
            Sizes = filter.FilterRootDictionaries.Select(frd => frd.RootDictionary.SizeType).
                Where(st => st != null).Select(st => st.ToEnum()).ToArray();

            InitialiseBoundingBox(filter.BoundingBox);
        }

        public SavedFilterInfo(BoundingBox boundingBox, 
            IEnumerable<Weezlabs.Storgage.Model.Enums.SizeType> sizeTypes,
            IEnumerable<Weezlabs.Storgage.Model.Enums.SpaceAccessType> accessTypes, 
            IEnumerable<Weezlabs.Storgage.Model.Enums.SpaceType> spaceTypes,
            Decimal? minRate, Decimal? maxRate, DateTimeOffset rentStartDate, 
            String location, String zipCode) 
                : base(boundingBox, sizeTypes, accessTypes, spaceTypes, minRate, maxRate)
        {
            RentStartDate = rentStartDate;
            Location = location;
            ZipCode = zipCode;
        }

        /// <summary>
        /// Validates filter model
        /// </summary>
        public override void Verify()
        {
            if (RentStartDate < DateTime.UtcNow)
            {
                throw new ArgumentOutOfRangeException(null, Messages.BadFilterRentStartDate);
            }

            if (ZipCode != null && ZipCode.Length > 10)
            {
                throw new ArgumentOutOfRangeException(null, String.Format(Messages.ZipCodeTooLong, 10));
            }

            if (Location != null && Location.Length > 100)
            {
                throw new ArgumentOutOfRangeException(null, String.Format(Messages.LocationTooLong, 100));
            }

            CheckEnumAttributesOnDuplication(AccessTypes, "access types");
            CheckEnumAttributesOnDuplication(SpaceTypes, "types");
            CheckEnumAttributesOnDuplication(Sizes, "sizes");

            base.Verify();

            if (BBox != null)
            {
                // no exceptions are expected on the next line convertion
                DbGeography polygon = DbGeography.PolygonFromText(BBox.ToString(), DbGeography.DefaultCoordinateSystemId);

                if (polygon.Area.HasValue && polygon.Area.Value > geographyMaxArea)
                {
                    throw new ArgumentOutOfRangeException(null, Messages.BoundingBoxAreaExceedsLimit);
                }
                else if (!polygon.Area.HasValue)
                {
                    throw new ArgumentOutOfRangeException(null, Messages.BoundingBoxMustBeSurface);
                }
            }
        }

        private void CheckEnumAttributesOnDuplication<T>(IEnumerable<T> collection, String propertyName) 
            where T : struct, IConvertible
        {
            if (collection.Distinct().Count() < collection.Count())
            {
                throw new ArgumentOutOfRangeException(null, String.Format(Messages.DuplicatesNotAllowed, propertyName));
            }
        }

        private void InitialiseBoundingBox(DbGeography coordinates)
        {
            if (coordinates != null)
            {
                if (coordinates.SpatialTypeName == "Polygon" && coordinates.PointCount == 5 && 
                    coordinates.StartPoint.Difference(coordinates.EndPoint).IsEmpty)
                {
                    BBox = new BoundingBox
                    {
                        TopLeftPoint = new GeoPoint(coordinates.PointAt(1)),
                        BottomRightPoint = new GeoPoint(coordinates.PointAt(3))
                    };
                }
            }
        }

        /// <summary>
        /// Restricts maximum geographic area of spaces search
        /// </summary>
        private const Double geographyMaxArea = 28030102663d;  // ~ 28 thousand square kilometres
    }
}