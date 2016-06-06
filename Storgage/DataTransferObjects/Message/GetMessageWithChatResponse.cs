namespace Weezlabs.Storgage.DataTransferObjects.Message
{   
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.Contracts;

    using Model.Enums;

    using Newtonsoft.Json;
           

    /// <summary>
    /// Information about message.
    /// </summary>
    public class GetMessageWithChatResponse : GetMessageResponse
    {
        /// <summary>
        /// Chat info.
        /// </summary>
        [Required]
        public GetChatResponse Chat { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GetMessageWithChatResponse()
        { }

        /// <summary>
        /// Creates DTO from model.
        /// </summary>
        /// <param name="message">Model object.</param>
        public GetMessageWithChatResponse(Model.Message message)
            : base(message)
        {
            Chat = new GetChatResponse(message.Chat);
        }
    }
}
