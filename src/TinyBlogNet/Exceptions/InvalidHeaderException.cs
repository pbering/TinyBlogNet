using System;
using System.Runtime.Serialization;

namespace TinyBlogNet.Exceptions
{
    [Serializable]
    public class InvalidHeaderException : Exception
    {
        public InvalidHeaderException()
        {
        }

        public InvalidHeaderException(string message) : base(message)
        {
        }

        public InvalidHeaderException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidHeaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}