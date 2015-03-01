using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StackoverflowOfflineDotNet.Models;
using Npgsql;
using NpgsqlTypes;

namespace StackoverflowOfflineDotNet.Service
{
    public static class SearchService
    {
        public static List<Result> Search(string keyword) 
        {
            List<Result> result = new List<Result>();

            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;User=postgres;Password=00fafage;Database=android;");
            conn.Open();

            NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT id, title FROM posts WHERE title <> '' AND body LIKE :keyword", conn);
            npgSqlCommand.Parameters.Add(new NpgsqlParameter("keyword", NpgsqlDbType.Text));
            npgSqlCommand.Parameters[0].Value = "%" + keyword + "%";

            NpgsqlDataReader dr = npgSqlCommand.ExecuteReader();
            while (dr.Read())
            {
                try
                {
                    result.Add(new Result(dr.GetInt32(0), dr.GetString(1)));
                }
                catch (Exception e) { }
            }

            conn.Close();

            return result;
        }
    }
}
