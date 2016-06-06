namespace Weezlabs.Storgage.Model.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Chat existed exception.
    /// </summary>
    [Serializable]
    public class BeginChatException : Exception
    {
        private const String PropertyName = "ChatId";

        private readonly Guid existedChatId;

        /// <summary>
        /// Existed chat identifier.
        /// </summary>
        public Guid ExistedChatId { get { return existedChatId; } }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BeginChatException() : base()
        { }

        /// <summary>
        /// Creates chat existed exception.
        /// </summary>
        /// <param name="existedChatId">Existed chat identifier.</param>
        public BeginChatException(Guid existedChatId) : base()
        {
            this.existedChatId = existedChatId;
        }

        /// <summary>
        /// Creates chat existed exception.
        /// </summary>
        /// <param name="message">Message.</param>
        public BeginChatException(String message) : base(message)
        {
        }

        /// <summary>
        /// Creates chat existed exception.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public BeginChatException(String message, Exception innerException) : base(message, innerException)
        { }

        /// <summary>
        /// Creates chat existed exception.
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Serialization context.</param>
        protected BeginChatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            existedChatId = new Guid(info.GetString(PropertyName));
        }

        /// <summary>
        /// Serialize extra data.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Serialization context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(PropertyName, existedChatId.ToString());
            base.GetObjectData(info, context);
        }
    }
}
