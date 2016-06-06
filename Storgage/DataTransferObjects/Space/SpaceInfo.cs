namespace Weezlabs.Storgage.DataTransferObjects.Space
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.Contracts;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// base DTO for space info.
    /// </summary>
    public abstract class SpaceInfo
    {
        /// <summary>
        /// Space title.
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Space description.
        /// </summary>
        public String Description { get; set; }        
        
        /// <summary>
        /// Short adress information
        /// </summary>
        public String ShortAddress { get; set; }
      

        /// <summary>
        /// Access type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public Model.Enums.SpaceAccessType Access { get; set; }

        /// <summary>
        /// Space size.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public Model.Enums.SizeType Size { get; set; }

        /// <summary>
        /// Space type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public Model.Enums.SpaceType Type { get; set; }

        /// <summary>
        /// Rate per month.
        /// </summary>
        [Range(1, 999)]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessageResourceType = typeof(Resources.Messages), ErrorMessageResourceName = "InvalidPriceFormat")]
        public Double Rate { get; set; }

        /// <summary>
        /// Zip Code
        /// </summary>
        public String ZipCode { get; set; }

        [Required]
        /// <summary>
        /// Longitude and latitude of space on map.
        /// </summary>
        public GeoPoint LocationOnMap { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected SpaceInfo()
        {
        }

        /// <summary>
        /// Creates SpaceInfo DTO from model.
        /// </summary>
        /// <param name="space">Model object.</param>
        protected SpaceInfo(Model.Space space)
        {
            Contract.Requires(space != null);
            Contract.Requires(space.PhotoLibraries != null);
           
            Title = space.Title;
            Description = space.Decription;
            Rate = (Double)space.Rate;
            Access = space.SpaceAccessType.ToEnum();
            Size = space.SizeType.ToEnum();
            Type = space.SpaceType.ToEnum();
            ZipCode = space.Zip == null ? (String)null : space.Zip.ZipCode;
            ShortAddress = space.ShortAddress;
            LocationOnMap = new GeoPoint(space.Location);
        }
    }
}
