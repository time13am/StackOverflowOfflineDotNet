using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackoverflowOfflineDotNet.Models
{
    public class Result
    {
        public Result(int id, string title)
        {
            this.id = id;
            this.title = title;
        }
        public int id { get; set; }
        public string title { get; set; }
    }

    public class Question
    {
        public int id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public List<Comment> comments { get; set; }

        public Question(int id, string title, string body, List<Comment> comments)
        {
            this.id = id;
            this.title = title;
            this.body = body;
            this.comments = comments;
        }

        public Question(int id,  string body, List<Comment> comments)
        {
            this.id = id;
            this.title = null;
            this.body = body;
            this.comments = comments;
        }
    }

    public class Comment
    {
        public String text { get; set; }

        public Comment(String text)
        {
            this.text = text;
        }
    }
}