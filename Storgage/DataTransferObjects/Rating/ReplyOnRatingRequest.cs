
namespace Weezlabs.Storgage.DataTransferObjects.Rating
{
    using System;
    using System.ComponentModel.DataAnnotations;
    
    /// <summary>
    /// Replyy on rating.
    /// </summary>
    public class ReplyOnRatingRequest
    {
        /// <summary>
        /// Reply message.
        /// </summary>
        [Required]
        [MaxLength(300)]
        public String Reply { get; set; }
    }
}
