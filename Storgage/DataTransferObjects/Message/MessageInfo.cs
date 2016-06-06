namespace Weezlabs.Storgage.DataTransferObjects.Message
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Message info.
    /// </summary>
    public class MessageInfo
    {               
        /// <summary>
        /// Text of message.
        /// </summary>
        [Required]
        [MaxLength(300)]
        public String Text { get; set; }        
    }
}
