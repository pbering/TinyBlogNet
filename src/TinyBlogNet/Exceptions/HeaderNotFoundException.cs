using System;
using System.Runtime.Serialization;

namespace TinyBlogNet.Exceptions
{
    [Serializable]
    public class HeaderNotFoundException : Exception
    {
        public HeaderNotFoundException()
        {
        }

        public HeaderNotFoundException(string message) : base(message)
        {
        }

        public HeaderNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected HeaderNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}