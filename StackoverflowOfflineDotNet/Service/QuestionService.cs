using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StackoverflowOfflineDotNet.Models;
using Npgsql;
using NpgsqlTypes;

namespace StackoverflowOfflineDotNet.Service
{
    public static class QuestionService
    {
        public static List<Question> process_page(int id)
        {
            var posts = new List<Question>();
            posts.Add(get_question(id));
            posts.AddRange(get_answers(id));

            return posts;
        }

        public static Question get_question(int id)
        {
            Question question = null;

            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;User=postgres;Password=00fafage;Database=android;");
            conn.Open();

            NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT title, body FROM posts WHERE id = :id", conn);
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("id", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters[0].Value = id;

            NpgsqlDataReader dr = npgSqlCommand.ExecuteReader();
            while (dr.Read())
                question = new Question(id, dr.GetString(0), dr.GetString(1), get_comments(id));

            conn.Close();

            return question;
        }

        public static List<Question> get_answers(int id)
        {
            var answers = new List<Question>();

            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;User=postgres;Password=00fafage;Database=android;");
            conn.Open();

            NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT body, id FROM posts WHERE parent_id = :id ORDER BY creation_date DESC", conn);
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("id", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters[0].Value = id;

            NpgsqlDataReader dr = npgSqlCommand.ExecuteReader();
            while (dr.Read())
            {
                try
                {
                    answers.Add(new Question(dr.GetInt32(1), dr.GetString(0), get_comments(id)));
                }
                catch (Exception e) { }
            }

            conn.Close();

            return answers;
        }

        public static List<Comment> get_comments(int id)
        {
            var comments = new List<Comment>();

            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;User=postgres;Password=00fafage;Database=android;");
            conn.Open();

            NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT text FROM comments WHERE post_id = :id ORDER BY creation_date ASC", conn);
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("id", NpgsqlDbType.Integer));
            npgSqlCommand.Parameters[0].Value = id;

            NpgsqlDataReader dr = npgSqlCommand.ExecuteReader();
            while (dr.Read())
            {
                try
                {
                    comments.Add(new Comment(dr.GetString(0)));
                }
                catch (Exception e) { }
            }

            conn.Close();

            return comments;
        }
    }
}