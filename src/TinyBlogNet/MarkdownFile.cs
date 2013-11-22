using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MarkdownSharp;
using TinyBlogNet.Exceptions;
using TinyBlogNet.IO;

namespace TinyBlogNet
{
    public class MarkdownFile
    {
        private readonly FileBase _file;
        private readonly Dictionary<string, string> _headers;
        private bool _isParsed;

        public MarkdownFile(FileBase file)
        {
            _file = file;
           _headers = new Dictionary<string, string>();
           
            Name = _file.Name.Replace(file.Extension, "");
            Modified = _file.Modified;
        }

        public string Name { get; internal set; }
        public string Body { get; internal set; }
        public DateTime Modified { get; internal set; }

        public virtual void Parse()
        {
            var parser = new Markdown();

            using (TextReader reader = new StreamReader(_file.OpenRead(), Encoding.UTF8))
            {
                var headerDone = false;
                var line = reader.ReadLine();

                if (string.IsNullOrEmpty(line))
                {
                    throw new ParseException("Empty line");
                }

                if (!line.Equals("---"))
                {
                    throw new ParseException("Invalid first line");
                }

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Equals("---"))
                    {
                        headerDone = true;

                        break;
                    }

                    var lineArray = line.Split(':');

                    if (lineArray.Length < 2)
                    {
                        throw new InvalidHeaderException("No : found");
                    }

                    if (lineArray.Length > 2)
                    {
                        throw new InvalidHeaderException("More than one : found");
                    }

                    _headers.Add(lineArray[0].Trim().ToLowerInvariant(), lineArray[1].Trim());
                }

                if (!headerDone && ((StreamReader)reader).EndOfStream)
                {
                    throw new ParseException("Headers not parsed yet but we are at the end of the stream");
                }

                if (headerDone && !_headers.Any())
                {
                    throw new InvalidHeaderException("No headers was found");
                }

                Body = parser.Transform(reader.ReadToEnd());

                _isParsed = true;
            }
        }

        public virtual string GetHeaderValue(string key, bool optional = false)
        {
            if (!_isParsed)
            {
                Parse();
            }

            string value;

            if (!_headers.TryGetValue(key, out value))
            {
                if (!optional)
                {
                    throw new HeaderNotFoundException(string.Format("'{0}' was not found", key));
                }
            }

            return value;
        }
    }
}