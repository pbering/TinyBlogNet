using System;
using System.Runtime.Serialization;

namespace TinyBlogNet.Exceptions
{
    [Serializable]
    public class InvalidHeaderValueException : Exception
    {
        public InvalidHeaderValueException()
        {
        }

        public InvalidHeaderValueException(string message) : base(message)
        {
        }

        public InvalidHeaderValueException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidHeaderValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}