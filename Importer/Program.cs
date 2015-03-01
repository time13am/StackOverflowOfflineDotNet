using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using System.IO;
using Npgsql;
using NpgsqlTypes;

namespace Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            Task task1 = Task.Factory.StartNew(() => import_comments());
            //Task task2 = Task.Factory.StartNew(() => import_posthistory());
            Task task3 = Task.Factory.StartNew(() => import_posts());
            Task.WaitAll(task1, task3);

            Console.WriteLine("All threads complete");
            Console.ReadLine();
        }

        static void import_comments()
        {
            Console.WriteLine("COMMENTS - STARTED");
            using (XmlReader reader = XmlReader.Create(new FileStream(@"d:\Temp\stackoverflow\Comments.xml", FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read)))
            {
                while (reader.Read())
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if ("row".Equals(reader.Name))
                            {
                                new Comment(reader).store_entry();
                            }
                            break;
                    }
            }
            Console.WriteLine("COMMENTS - FINISHED");
        }
        static void import_posthistory()
        {
            Console.WriteLine("POSTHISTORY - STARTED");
            using (XmlReader reader = XmlReader.Create(new FileStream(@"d:\Temp\stackoverflow\PostHistory.xml", FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read)))
            {
                while (reader.Read())
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if ("row".Equals(reader.Name))
                            {
                                new PostHistory(reader).store_entry();
                            }
                            break;
                    }
            }
            Console.WriteLine("POSTHISTORY - FINISHED");
        }
        static void import_posts()
        {
            Console.WriteLine("POSTS - STARTED");
            using (XmlReader reader = XmlReader.Create(new FileStream(@"d:\Temp\stackoverflow\Posts.xml", FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read)))
            {
                while (reader.Read())
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if ("row".Equals(reader.Name))
                            {
                                new Post(reader).store_entry();
                            }
                            break;
                    }
            }
            Console.WriteLine("POSTS - FINISHED");
        }
    }

    public class Comment
    {
        public String Id;
        public String PostId;
        public String Score;
        public String Text;
        public String CreationDate;
        public String UserDisplayName;
        public String UserId;

        public Comment(XmlReader reader)
        {
            try
            {
                this.Id = reader.GetAttribute("Id");
                this.PostId = reader.GetAttribute("PostId");
                this.Score = reader.GetAttribute("Score");
                this.Text = reader.GetAttribute("Text");
                this.CreationDate = reader.GetAttribute("CreationDate");
                this.UserDisplayName = reader.GetAttribute("UserDisplayName");
                this.UserId = reader.GetAttribute("UserId");
            }
            catch (Exception e) { } 
        }

        public void store_entry()
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;User=postgres;Password=00fafage;Database=android;");
            conn.Open();

            NpgsqlCommand npgSqlCommand = new NpgsqlCommand("INSERT INTO comments(id, post_id, score, text, creation_date, user_display_name, user_id) VALUES (:id, :post_id, :score, :text, :creation_date, :user_display_name, :user_id);", conn);

            npgSqlCommand.Parameters.Add(new NpgsqlParameter("id", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("post_id", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("score", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("text", NpgsqlDbType.Text));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("creation_date", NpgsqlDbType.Timestamp));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("user_display_name", NpgsqlDbType.Text));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("user_id", NpgsqlDbType.Integer));
            
            npgSqlCommand.Parameters[0].Value = this.Id;
            npgSqlCommand.Parameters[1].Value = this.PostId;
            npgSqlCommand.Parameters[2].Value = this.Score;
            npgSqlCommand.Parameters[3].Value = this.Text;
            try { npgSqlCommand.Parameters[4].Value = NpgsqlTimeStamp.Parse(this.CreationDate.Replace('T', ' ')); }
            catch (Exception e) { npgSqlCommand.Parameters[4].Value = null; }
            npgSqlCommand.Parameters[5].Value = this.UserDisplayName;
            npgSqlCommand.Parameters[6].Value = this.UserId;

            NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();

            conn.Close();
        }
    }

    public class PostHistory
    {
        public String Id;
        public String PostHistoryTypeId;
        public String PostId;
        public String RevisionGUID;
        public String CreationDate;
        public String UserId;
        public String UserDisplayName;
        public String Comment;
        public String Text;

        public PostHistory(XmlReader reader)
        {
            try
            {
                this.Id                 = reader.GetAttribute("Id");
                this.PostHistoryTypeId  = reader.GetAttribute("PostHistoryTypeId");
                this.PostId             = reader.GetAttribute("PostId");
                this.RevisionGUID       = reader.GetAttribute("RevisionGUID");
                this.CreationDate       = reader.GetAttribute("CreationDate");
                this.UserId             = reader.GetAttribute("UserId");
                this.UserDisplayName    = reader.GetAttribute("UserDisplayName");
                this.Comment            = reader.GetAttribute("Comment");
                this.Text               = reader.GetAttribute("Text");
            }
            catch (Exception e) { }     
        }

        public void store_entry()
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;User=postgres;Password=00fafage;Database=android;");
            conn.Open();

            NpgsqlCommand npgSqlCommand = new NpgsqlCommand("INSERT INTO posthistory(id, post_history_type_id, post_id, revision_guid, creation_date, user_id, user_display_name, comment, text) VALUES (:id, :post_history_type_id, :post_id, :revision_guid, :creation_date, :user_id, :user_display_name, :comment, :text);", conn);

            npgSqlCommand.Parameters.Add(new NpgsqlParameter("id", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("post_history_type_id", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("post_id", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("revision_guid", NpgsqlDbType.Text));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("creation_date", NpgsqlDbType.Timestamp));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("user_id", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("user_display_name", NpgsqlDbType.Text));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("comment", NpgsqlDbType.Text));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("text", NpgsqlDbType.Text));

            npgSqlCommand.Parameters[0].Value = this.Id;
            npgSqlCommand.Parameters[1].Value = this.PostHistoryTypeId;
            npgSqlCommand.Parameters[2].Value = this.PostId;
            npgSqlCommand.Parameters[3].Value = this.RevisionGUID;
            try { npgSqlCommand.Parameters[4].Value = NpgsqlTimeStamp.Parse(this.CreationDate.Replace('T', ' ')); }
            catch (Exception e) { npgSqlCommand.Parameters[4].Value = null; }
            npgSqlCommand.Parameters[5].Value = this.UserId;
            npgSqlCommand.Parameters[6].Value = this.UserDisplayName;
            npgSqlCommand.Parameters[7].Value = this.Comment;
            npgSqlCommand.Parameters[8].Value = this.Text;

            NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();

            conn.Close();
        }
    }

    public class Post
    {
        public String AcceptedAnswerId;
        public String AnswerCount;
        public String Body;
        public String CommentCount;
        public String CreationDate;
        public String FavoriteCount;
        public String Id;
        public String LastActivityDate;
        public String LastEditDate;
        public String LastEditorUserId;
        public String OwnerUserId;
        public String PostTypeId;
        public String Score;
        public String Tags;
        public String Title;
        public String ViewCount;
        public String CommunityOwnedDate;
        public String ParentId;
        public String LastEditorDisplayName;
        public String ClosedDate;
        public String OwnerDisplayName;

        public Post(XmlReader reader)
        {
            try
            {
                this.Id                             = reader.GetAttribute("Id");
                this.PostTypeId                     = reader.GetAttribute("PostTypeId");
                this.AcceptedAnswerId               = reader.GetAttribute("AcceptedAnswerId"); 
                this.ParentId                       = reader.GetAttribute("ParentId"); 
                this.CreationDate                   = reader.GetAttribute("CreationDate");
                this.Score                          = reader.GetAttribute("Score"); 
                this.ViewCount                      = reader.GetAttribute("ViewCount"); 
                this.Body                           = reader.GetAttribute("Body");
                this.OwnerUserId                    = reader.GetAttribute("OwnerUserId");
                this.OwnerDisplayName               = reader.GetAttribute("OwnerDisplayName");
                this.LastEditorUserId               = reader.GetAttribute("LastEditorUserId");
                this.LastEditorDisplayName          = reader.GetAttribute("LastEditorDisplayName");
                this.LastEditDate                   = reader.GetAttribute("LastEditDate");
                this.LastActivityDate               = reader.GetAttribute("LastActivityDate");
                this.Title                          = reader.GetAttribute("Title");
                this.Tags                           = reader.GetAttribute("Tags");
                this.AnswerCount                    = reader.GetAttribute("AnswerCount");
                this.CommentCount                   = reader.GetAttribute("CommentCount");
                this.FavoriteCount                  = reader.GetAttribute("FavoriteCount");
                this.ClosedDate                     = reader.GetAttribute("ClosedDate");
                this.CommunityOwnedDate             = reader.GetAttribute("CommunityOwnedDate");
            }
            catch (Exception e) { }             
        }

        public void store_entry()
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;User=postgres;Password=00fafage;Database=android;");
            conn.Open();

            NpgsqlCommand npgSqlCommand = new NpgsqlCommand("INSERT INTO posts(id, post_type_id, accepted_answer_id, parent_id, score, view_count, body, owner_user_id, owner_display_name, last_editor_user_id, last_editor_display_name, title, answer_count, comment_count, tags, favourite_count, creation_date, last_edit_date, last_activity_date, closed_date, community_owned_date) VALUES (:id, :post_type_id, :accepted_answer_id, :parent_id, :score, :view_count, :body, :owner_user_id, :owner_display_name, :last_editor_user_id, :last_editor_display_name, :title, :answer_count, :comment_count, :tags, :favourite_count, :creation_date, :last_edit_date, :last_activity_date, :closed_date, :community_owned_date);", conn);
            
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("id", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("post_type_id", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("accepted_answer_id", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("parent_id", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("score", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("view_count", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("body", NpgsqlDbType.Text));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("owner_user_id", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("owner_display_name", NpgsqlDbType.Text));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("last_editor_user_id", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("last_editor_display_name", NpgsqlDbType.Text));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("title", NpgsqlDbType.Text));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("answer_count", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("comment_count", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("tags", NpgsqlDbType.Text));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("favourite_count", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("creation_date", NpgsqlDbType.Timestamp));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("last_edit_date", NpgsqlDbType.Timestamp));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("last_activity_date", NpgsqlDbType.Timestamp));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("closed_date", NpgsqlDbType.Timestamp));
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("community_owned_date", NpgsqlDbType.Timestamp));

            npgSqlCommand.Parameters[0].Value   = this.Id;
            npgSqlCommand.Parameters[1].Value   = this.PostTypeId;
            npgSqlCommand.Parameters[2].Value   = this.AcceptedAnswerId;
            npgSqlCommand.Parameters[3].Value   = this.ParentId;
            npgSqlCommand.Parameters[4].Value   = this.Score;
            npgSqlCommand.Parameters[5].Value   = this.ViewCount;
            npgSqlCommand.Parameters[6].Value   = this.Body;
            npgSqlCommand.Parameters[7].Value   = this.OwnerUserId;
            npgSqlCommand.Parameters[8].Value   = this.OwnerDisplayName;
            npgSqlCommand.Parameters[9].Value   = this.LastEditorUserId;
            npgSqlCommand.Parameters[10].Value  = this.LastEditorDisplayName;
            npgSqlCommand.Parameters[11].Value  = this.Title;
            npgSqlCommand.Parameters[12].Value  = this.AnswerCount;
            npgSqlCommand.Parameters[13].Value  = this.CommentCount;
            npgSqlCommand.Parameters[14].Value  = this.Tags;
            npgSqlCommand.Parameters[15].Value  = this.FavoriteCount;

            try { npgSqlCommand.Parameters[16].Value = NpgsqlTimeStamp.Parse(this.CreationDate.Replace('T', ' ')); } 
            catch (Exception e) { npgSqlCommand.Parameters[16].Value = null; }
            try { npgSqlCommand.Parameters[17].Value = NpgsqlTimeStamp.Parse(this.LastEditDate.Replace('T', ' ')); } 
            catch (Exception e) { npgSqlCommand.Parameters[17].Value = null; }
            try { npgSqlCommand.Parameters[18].Value = NpgsqlTimeStamp.Parse(this.LastActivityDate.Replace('T', ' ')); }
            catch (Exception e) { npgSqlCommand.Parameters[18].Value = null; }
            try { npgSqlCommand.Parameters[19].Value = NpgsqlTimeStamp.Parse(this.ClosedDate.Replace('T', ' ')); }
            catch (Exception e) { npgSqlCommand.Parameters[19].Value = null; }
            try { npgSqlCommand.Parameters[20].Value = NpgsqlTimeStamp.Parse(this.CommunityOwnedDate.Replace('T', ' ')); }
            catch (Exception e) { npgSqlCommand.Parameters[20].Value = null; }
            
            NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();

            conn.Close();
        }
    }
}
