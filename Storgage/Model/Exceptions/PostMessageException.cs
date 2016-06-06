namespace Weezlabs.Storgage.Model.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class PostMessageException : Exception
    {
        public PostMessageException() : base()
        { }

        public PostMessageException(String message) : base(message)
        { }

        public PostMessageException(String message, Exception innerException) : base(message, innerException)
        { }

        protected PostMessageException(SerializationInfo info, StreamingContext context) : base (info, context)
        { }
    }
}
