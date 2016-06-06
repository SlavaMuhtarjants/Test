namespace Weezlabs.Storgage.DataTransferObjects.Filter
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Model.Enums;
    using Space;

    public class CreateFilterRequest
    {
        [Required]
        public SpaceAccessType[] AccessTypes { get; set; }

        [Required]
        public BoundingBox BoundingBox{ get; set; }

        [Required]
        [MaxLength(100)]
        public String Location { get; set; }

        public Decimal? MaxPrice { get; set; }

        public Decimal? MinPrice { get; set; }

        [Required]
        public DateTimeOffset RentStartDate { get; set; }

        [Required]
        public SizeType[] SizeTypes { get; set; }

        [Required]
        public SpaceType[] Types { get; set; }

        [RegularExpression(@"^\d{5,10}$", ErrorMessage = "Zip code must contain from 5 to 10 unsigned integers")]
        public String ZipCode { get; set; }
    }
}