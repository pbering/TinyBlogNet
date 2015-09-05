using System.Collections.Generic;
using TinyBlogNet;

namespace MvcBlog.Models
{
    public class HomeModel
    {
        public HomeModel(IEnumerable<Post> posts)
        {
            Posts = posts;
        }

        public IEnumerable<Post> Posts { get; }
    }
}