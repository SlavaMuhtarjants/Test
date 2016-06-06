namespace Weezlabs.Storgage.DataTransferObjects.Space
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Web.Configuration;

    /// <summary>
    /// Contains information about photo.
    /// </summary>
    public class Photo
    {
        /// <summary>
        /// Photo identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Link to S3 bucket with storgage photos.
        /// </summary>
        public String Link { get; set; }

        /// <summary>
        /// Name of file.
        /// </summary>
        public String FileName { get; set; }

        /// <summary>
        /// True - photo is default.
        /// </summary>
        public Boolean IsDefault { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Photo()
        {
        }

        /// <summary>
        /// Creates photo from model.
        /// </summary>
        /// <param name="photo">Model object.</param>
        public Photo(Model.PhotoLibrary photo)
        {
            Contract.Requires(photo != null);

            Id = photo.Id;
            Link = WebConfigurationManager.AppSettings["s3spacesImagesUrl"];
            FileName = photo.Link;
            IsDefault = photo.Space.DefaultPhotoID == photo.Id;
        }
    }
}