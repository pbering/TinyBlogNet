using System.Collections.Generic;
using System.Text;
using TinyBlogNet;
using TinyBlogNet.Pipeline;

namespace Blog.Pipeline
{
    internal class DefaultLayout : Layout
    {
        public DefaultLayout(string siteName)
        {
            Template = "<!DOCTYPE html>\n" +
                        "<html lang=\"en-US\">" +
                        "<head>" +
                        "<meta charset=\"utf-8\">" +
                        "<meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">" +
                        "<title>{0} - " + siteName + "</title>" +
                        "<link rel=\"icon\" type=\"image/png\" href=\"/content/favicon.png\" />" +
                        "<link href=\"/content/blog.css\" rel=\"stylesheet\" type=\"text/css\" media=\"all\" />" +
                        "<link rel=\"alternate\" type=\"application/rss+xml\" title=\""+ siteName + "\" href=\"/rss.xml\" />" +
                        "</head>" +
                        "<body>" +
                        "<nav><a href=\"/\">" + siteName + "</a> | <a href=\"/rss.xml\">rss</a></nav>" +
                        "{1}" +
                        "<footer><p>reach out: <a href=\"https://twitter.com/pbering\">twitter.com/pbering</a>, <a href=\"https://github.com/pbering\">github.com/pbering</a></p></footer>" +
                        "</body>" +
                        "</html>";
        }

        public override string Template { get; }

        public static string GetPostListFragment(IEnumerable<Post> posts)
        {
            var content = new StringBuilder();

            foreach (var post in posts)
            {
                content.AppendFormat("<h1><a href=\"{0}\">{1}</a></h1>", post.Url, post.Title);
                content.AppendFormat("<p>{0}</p>", post.Summary);
                content.Append(GetPostInfoFragment(post));
            }

            return content.ToString();
        }

        public static string GetPostInfoFragment(Post post)
        {
            var content = new StringBuilder();

            content.AppendFormat("<code>Posted {0}, tagged: ", post.Published.ToHumaneString());

            for (var i = 0; i < post.Tags.Count; i++)
            {
                var tag = post.Tags[i];

                content.AppendFormat("<a href=\"{0}\">{1}</a>{2}", tag.Url, tag.Name, i < post.Tags.Count - 1 ? ", " : "");
            }

            content.Append("</code>");

            return content.ToString();
        }
    }
}