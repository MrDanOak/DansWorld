using DansWorld.Common.Enums;
using DansWorld.Common.IO;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DansWorld.Server.Data
{
    public class Database
    {
        private string Server { get; set; }
        private string Password { get; set; }
        private string User { get; set; }
        private string DatabaseName { get; set; }
        private string ConnectionString { get; set; }

        public Database()
        {
            Server = "idanscott.co.uk";
            DatabaseName = "DansWorld";
            User = "DansWorld";
            Password = "uL2qDxZcInz7PU72";
            ConnectionString = "Server=" + Server + "; Database=" + DatabaseName +
                "; Uid=" + User + "; Pwd=" + Password + ";";
        }

        public DataTable Select(string what, string from, string col = "", string whereVal = "")
        {
            using (MySqlConnection Connection = new MySqlConnection(ConnectionString))
            {
                Connection.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT " + what + " FROM " + from + (col != "" ? " WHERE " + col + " = '" + whereVal + "'" : ""), Connection);
                DataTable dt = new DataTable();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
                Connection.Close();
                return dt;
            }
        }

        public int Insert(string query)
        {
            return Query(query);
        }

        public int Query(string query)
        {
            using (MySqlConnection Connection = new MySqlConnection(ConnectionString))
            {
                Connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, Connection);
                int returnRows = cmd.ExecuteNonQuery();
                Connection.Close();
                return returnRows;
            }
        }

        public int Update(string query)
        {
            return Query(query);
        }

        public int Update(string table, string newVal, string column, string whereColumn, string whereVal)
        {
            return Query("UPDATE '" + table + "' SET " + column + " = '" + newVal + "' WHERE " + whereColumn + " = '" + whereVal + "'");
        }

        public int Update(string table, int newVal, string column, string whereColumn, string whereVal)
        {
            return Query("UPDATE '" + table + "' SET " + column + " = " + newVal + " WHERE " + whereColumn + " = '" + whereVal + "'");
        }

        public int Update(string table, int newVal, string column, string whereColumn, int whereVal)
        {
            return Query("UPDATE '" + table + "' SET " + column + " = " + newVal + " WHERE " + whereColumn + " = " + whereVal);
        }

        public int Update(string table, string newVal, string column, string whereColumn, int whereVal)
        {
            return Query("UPDATE '" + table + "' SET " + column + " = '" + newVal + "' WHERE " + whereColumn + " = " + whereVal);
        }

        public int Delete(string query)
        {
            return Query(query);
        }

        public int Delete(string table, string column, string val)
        {
            return Query("DELETE FROM '" + table + "' WHERE " + column + " = '" + val + "'");
        }

        public int Delete(string table, string column, int val)
        {
            return Query("DELETE FROM '" + table + "' WHERE " + column + " = " + val);
        }
    }
}
