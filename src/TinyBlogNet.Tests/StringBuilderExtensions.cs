using System;
using System.Text;

namespace TinyBlogNet.Tests
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendNewLine(this StringBuilder builder, string value)
        {
            builder.AppendFormat("{0}{1}", value, Environment.NewLine);
            
            return builder;
        }
    }
}