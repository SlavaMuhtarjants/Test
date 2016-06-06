namespace Weezlabs.Storgage.DataTransferObjects.Filter
{
    using System;
    using System.Collections.Generic;

    using Model.Enums;
    using Space;

    using NUnit.Framework;

    /// <summary>
    /// Contains information about user filter for spaces.
    /// </summary>
    public class FilterInfo : FilterBase
    {
        /// <summary>
        /// Search spaces by this location + Max. distance.
        /// </summary>
        public GeoPoint Location { get; set; }    
       
        /// <summary>
        /// Max. distance in miles to space.
        /// </summary>
        public Double? MaxDistance { get; set; }

        /// <summary>
        /// Date a space gets free from
        /// </summary>
        public DateTimeOffset? AvailableSince { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FilterInfo()
        {
        }

        /// <summary>
        /// Create FilterInfo instance.
        /// </summary>      
        /// <param name="location">Geo point.</param>
        /// <param name="maxDistance">Max/ distance to geo point.</param>
        /// <param name="bbox">Bounding box of map.</param>
        /// <param name="sizeTypes">Size types.</param>
        /// <param name="accessTypes">Access types.</param>
        /// <param name="spaceTypes">Space types.</param>
        /// <param name="minRate">Min. rate.</param>
        /// <param name="maxRate">Max. rate.</param>
        public FilterInfo(GeoPoint location, 
            Double? maxDistance,
            BoundingBox bbox,
            IEnumerable<SizeType> sizeTypes,
            IEnumerable<SpaceAccessType> accessTypes,
            IEnumerable<SpaceType> spaceTypes,
            Decimal? minRate,
            Decimal? maxRate
            ) : base(bbox, sizeTypes, accessTypes, spaceTypes, minRate, maxRate)
        {            
            Location = location;
            MaxDistance = maxDistance;
        }

        /// <summary>
        /// Create FilterInfo instance.
        /// </summary>
        /// <param name="id">Filter identifier.</param>        
        /// <param name="location">Geo point.</param>
        /// <param name="maxDistance">Max/ distance to geo point.</param>
        /// <param name="bbox">Bounding box of map.</param>
        /// <param name="sizeTypes">Size types.</param>
        /// <param name="accessTypes">Access types.</param>
        /// <param name="spaceTypes">Space types.</param>
        /// <param name="minRate">Min. rate.</param>
        /// <param name="maxRate">Max. rate.</param>
        public FilterInfo(Guid id,           
            GeoPoint location,
            Double? maxDistance,
            BoundingBox bbox,
            IEnumerable<SizeType> sizeTypes,
            IEnumerable<SpaceAccessType> accessTypes,
            IEnumerable<SpaceType> spaceTypes,
            Decimal? minRate,
            Decimal? maxRate)
            : this(location, maxDistance, bbox, sizeTypes, accessTypes, spaceTypes, minRate, maxRate)
        {
            Id = id;
        }

        /// <summary>
        /// Verify filterInfo model
        /// </summary>
        public override void Verify()
        {
            if (Location != null)
            {
                if (Location.Latitude < minLatitude || Location.Latitude > maxLatitude)
                {
                    throw new ArgumentOutOfRangeException(null,
                        String.Format(Resources.Messages.InvalidLatitude, Location.Latitude, minLatitude, maxLatitude));
                }

                if (Location.Longitude < minLongitude || Location.Longitude > maxLongitude)
                {
                    throw new ArgumentOutOfRangeException(null,
                        String.Format(Resources.Messages.InvalidLongitude, Location.Longitude, minLongitude, maxLongitude));
                }
            }

            if (MaxDistance != null && MaxDistance < Double.Epsilon)
            {
                throw new ArgumentOutOfRangeException(null, Resources.Messages.InvalidDistance);
            }

            base.Verify();
        }
        
        [TestFixture]
        class FilterInfoTest
        {
            [Test]
            public void TestFilterInfoConstructor()
            {
                var id = Guid.NewGuid();               
                var location = new GeoPoint { Latitude = 47.5678, Longitude = 30.1234 };
                var maxDistance = 2.5;
                var bbox = new BoundingBox
                {
                    TopLeftPoint = location,
                    BottomRightPoint = new GeoPoint { Longitude = location.Longitude - 0.0001, Latitude = location.Latitude - 0.0002 }
                };

                var sizeTypes = new SizeType[] { SizeType.Large, SizeType.XLarge };
                var accessTypes = new SpaceAccessType[] { SpaceAccessType.Limited };
                var spaceTypes = new SpaceType[] { SpaceType.Outdoor };

                var minRate = (Decimal) 50.0;
                var maxRate = (Decimal) 60.0;

                var filterInfo =
                    new FilterInfo(id, location, maxDistance, bbox, sizeTypes, accessTypes, 
                        spaceTypes, minRate, maxRate);

                Assert.AreEqual(id, filterInfo.Id);              
                Assert.AreEqual(maxDistance, filterInfo.MaxDistance);
                Assert.AreEqual(maxRate, filterInfo.MaxRate);
                Assert.AreEqual(minRate, filterInfo.MinRate);
                Assert.AreEqual(location.Latitude, filterInfo.Location.Latitude);
                Assert.AreEqual(location.Longitude, filterInfo.Location.Longitude);
                Assert.AreEqual(bbox.TopLeftPoint.Longitude, filterInfo.BBox.TopLeftPoint.Longitude);
                Assert.AreEqual(bbox.TopLeftPoint.Latitude, filterInfo.BBox.TopLeftPoint.Latitude);
                Assert.AreEqual(bbox.BottomRightPoint.Latitude, filterInfo.BBox.BottomRightPoint.Latitude);
                Assert.AreEqual(bbox.BottomRightPoint.Longitude, filterInfo.BBox.BottomRightPoint.Longitude);
            }
        }
    }
}