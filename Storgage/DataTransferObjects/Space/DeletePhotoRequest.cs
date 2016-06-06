namespace Weezlabs.Storgage.DataTransferObjects.Space
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Request model for deleting photo of space.
    /// </summary>
    public class DeletePhotoRequest
    {
        /// <summary>
        /// Array of photo ids for delete.
        /// </summary>
        [Required]
        public Guid[] PhotoIds { get; set; }
    }
}
