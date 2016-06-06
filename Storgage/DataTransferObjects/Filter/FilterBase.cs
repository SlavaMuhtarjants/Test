namespace Weezlabs.Storgage.DataTransferObjects.Filter
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;

    using Model.Enums;
    using Space;

    /// <summary>
    /// Contains information about user filter for spaces.
    /// </summary>
    public abstract class FilterBase
    {
        /// <summary>
        /// Filter identifier.
        /// </summary>
        public Guid Id { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        /// <summary>
        /// Search spaces by these sizes.
        /// </summary>
        public IEnumerable<SizeType> Sizes { get; set; }

        /// <summary>
        /// Search by bbox.
        /// </summary>
        public BoundingBox BBox { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        /// <summary>
        /// Search by access types.
        /// </summary>
        public IEnumerable<SpaceAccessType> AccessTypes { get; set; }

        /// <summary>
        /// Min. rate
        /// </summary>
        public Decimal? MinRate { get; set; }

        /// <summary>
        /// Max. rate.
        /// </summary>
        public Decimal? MaxRate { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        /// <summary>
        /// Space types to search.
        /// </summary>
        public IEnumerable<SpaceType> SpaceTypes { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FilterBase()
        {
        }

        /// <summary>
        /// Creates FilterBase instance.
        /// </summary>      
        /// <param name="bbox">Bounding box of map.</param>
        /// <param name="sizeTypes">Size types.</param>
        /// <param name="accessTypes">Access types.</param>
        /// <param name="spaceTypes">Space types.</param>
        /// <param name="minRate">Min. rate.</param>
        /// <param name="maxRate">Max. rate.</param>
        internal FilterBase(BoundingBox boundingBox, IEnumerable<SizeType> sizeTypes,
            IEnumerable<SpaceAccessType> accessTypes, IEnumerable<SpaceType> spaceTypes,
            Decimal? minRate, Decimal? maxRate)
        {
            BBox = boundingBox;
            Sizes = sizeTypes;
            SpaceTypes = spaceTypes;
            AccessTypes = accessTypes;
            MinRate = minRate;
            MaxRate = maxRate;
        }

        /// <summary>
        /// Verify filterInfo model
        /// </summary>
        public virtual void Verify()
        {
            if (BBox != null)
            {
                if (BBox.TopLeftPoint.Latitude < minLatitude || BBox.TopLeftPoint.Latitude > maxLatitude)
                {
                    throw new ArgumentOutOfRangeException(null,
                        String.Format(Resources.Messages.InvalidLatitude, BBox.TopLeftPoint.Latitude, minLatitude,
                            maxLatitude));
                }

                if (BBox.TopLeftPoint.Longitude < minLongitude || BBox.TopLeftPoint.Longitude > maxLongitude)
                {
                    throw new ArgumentOutOfRangeException(null,
                        String.Format(Resources.Messages.InvalidLongitude, BBox.TopLeftPoint.Longitude, minLongitude,
                            maxLongitude));
                }

                if (BBox.BottomRightPoint.Latitude < minLatitude || BBox.BottomRightPoint.Latitude > maxLatitude)
                {
                    throw new ArgumentOutOfRangeException(null,
                        String.Format(Resources.Messages.InvalidLatitude, BBox.BottomRightPoint.Latitude, minLatitude,
                            maxLatitude));
                }

                if (BBox.BottomRightPoint.Longitude < minLongitude || BBox.BottomRightPoint.Longitude > maxLongitude)
                {
                    throw new ArgumentOutOfRangeException(null,
                        String.Format(Resources.Messages.InvalidLongitude, BBox.BottomRightPoint.Longitude, minLongitude,
                            maxLongitude));
                }

                if (BBox.TopLeftPoint.Latitude <= BBox.BottomRightPoint.Latitude)
                {
                    throw new ArgumentOutOfRangeException(null, Resources.Messages.InvalidLatitudeDifference);
                }

                if (BBox.TopLeftPoint.Longitude == BBox.BottomRightPoint.Longitude)
                {
                    throw new ArgumentOutOfRangeException(null, Resources.Messages.EqualLongitudes);
                }
            }

            if (MinRate < Decimal.Zero)
            {
                throw new ArgumentOutOfRangeException(null, Resources.Messages.InvalidSpaceMinRate);
            }

            if (MaxRate < Decimal.Zero)
            {
                throw new ArgumentOutOfRangeException(null, Resources.Messages.InvalidSpaceMaxRate);
            }

            if (MinRate > MaxRate)
            {
                throw new ArgumentOutOfRangeException(null, Resources.Messages.InvalidSpaceRateRange);
            }
        }

        protected const Double minLongitude = -180.0;
        protected const Double minLatitude = -90.0;
        protected const Double maxLongitude = 180.0;
        protected const Double maxLatitude = 90.0;
    }
}