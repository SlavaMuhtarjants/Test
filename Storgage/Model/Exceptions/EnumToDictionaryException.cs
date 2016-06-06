namespace Weezlabs.Storgage.Model.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Enum to dictionary exception.
    /// </summary>
    [Serializable]
    public class EnumToDictionaryException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public EnumToDictionaryException() : base()
        { }

        /// <summary>
        /// Creates Enum to dictionary exception.
        /// </summary>
        /// <param name="message">Message.</param>
        public EnumToDictionaryException(String message) : base(message)
        { }

        /// <summary>
        /// Creates Enum to dictionary exception.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exeption.</param>
        public EnumToDictionaryException(String message, Exception innerException) : base(message, innerException)
        { }

        /// <summary>
        /// Creates Enum to dictionary exception.
        /// </summary>
        /// <param name="info">Info to serialize.</param>
        /// <param name="context">Serializable context.</param>
        protected EnumToDictionaryException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
