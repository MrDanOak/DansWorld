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
        /// <summary>
        /// IP address of the MySQL server
        /// </summary>
        private string Server { get; set; }
        /// <summary>
        /// Password for the MySQL server
        /// </summary>
        private string Password { get; set; }
        /// <summary>
        /// Username for the MySQL database
        /// </summary>
        private string User { get; set; }
        /// <summary>
        /// Name of the Database in the MySQL server
        /// </summary>
        private string DatabaseName { get; set; }
        /// <summary>
        /// A string that defines the connection parameters for the database connection
        /// </summary>
        private string ConnectionString { get; set; }

        /// <summary>
        /// Default constructor for the database object
        /// </summary>
        public Database()
        {
            Server = "idanscott.co.uk";
            DatabaseName = "DansWorld";
            User = "DansWorld";
            Password = "uL2qDxZcInz7PU72";
            ConnectionString = "Server=" + Server + "; Database=" + DatabaseName +
                "; Uid=" + User + "; Pwd=" + Password + ";";
        }

        /// <summary>
        /// SELECT * FROM..... SQL Query
        /// </summary>
        /// <param name="what">What're you selecting</param>
        /// <param name="from">From what table</param>
        /// <param name="col">Where what column</param>
        /// <param name="whereVal">Equals what value</param>
        /// <returns>A datatable with the results</returns>
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

        /// <summary>
        /// SELECT * FROM..... SQL Query
        /// </summary>
        /// <param name="what">What're you selecting</param>
        /// <param name="from">From what table</param>
        /// <param name="col">Where what column</param>
        /// <param name="whereVal">Equals what value</param>
        /// <returns>A datatable with the results</returns>
        public DataTable Select(string what, string from, string col, int whereVal)
        {
            using (MySqlConnection Connection = new MySqlConnection(ConnectionString))
            {
                Connection.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT " + what + " FROM " + from + (col != "" ? " WHERE " + col + " = " + whereVal + "" : ""), Connection);
                DataTable dt = new DataTable();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
                Connection.Close();
                return dt;
            }
        }

        /// <summary>
        /// INSERT INTO.... Query
        /// </summary>
        /// <param name="query">Built query</param>
        /// <returns>number of rows affected</returns>
        public int Insert(string query)
        {
            return Query(query);
        }

        /// <summary>
        /// Generic query
        /// </summary>
        /// <param name="query">Query to run</param>
        /// <returns>number of rows affected</returns>
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

        /// <summary>
        /// Generic query, doesn't have to be update, but it helps with code clarity
        /// </summary>
        /// <param name="query">built query</param>
        /// <returns>Number of rows affected</returns>
        public int Update(string query)
        {
            return Query(query);
        }

        /// <summary>
        /// Update statement builder
        /// </summary>
        /// <param name="table">table to update</param>
        /// <param name="newVal">new value</param>
        /// <param name="column">for column</param>
        /// <param name="whereColumn">where this column</param>
        /// <param name="whereVal">eqauls this value</param>
        /// <returns>number of rows affected</returns>
        public int Update(string table, string newVal, string column, string whereColumn, string whereVal)
        {
            return Query("UPDATE '" + table + "' SET " + column + " = '" + newVal + "' WHERE " + whereColumn + " = '" + whereVal + "'");
        }

        /// <summary>
        /// Update statement builder
        /// </summary>
        /// <param name="table">table to update</param>
        /// <param name="newVal">new value</param>
        /// <param name="column">for column</param>
        /// <param name="whereColumn">where this column</param>
        /// <param name="whereVal">eqauls this value</param>
        /// <returns>number of rows affected</returns>
        public int Update(string table, int newVal, string column, string whereColumn, string whereVal)
        {
            return Query("UPDATE '" + table + "' SET " + column + " = " + newVal + " WHERE " + whereColumn + " = '" + whereVal + "'");
        }

        /// <summary>
        /// Update statement builder
        /// </summary>
        /// <param name="table">table to update</param>
        /// <param name="newVal">new value</param>
        /// <param name="column">for column</param>
        /// <param name="whereColumn">where this column</param>
        /// <param name="whereVal">eqauls this value</param>
        /// <returns>number of rows affected</returns>
        public int Update(string table, int newVal, string column, string whereColumn, int whereVal)
        {
            return Query("UPDATE '" + table + "' SET " + column + " = " + newVal + " WHERE " + whereColumn + " = " + whereVal);
        }

        /// <summary>
        /// Update statement builder
        /// </summary>
        /// <param name="table">table to update</param>
        /// <param name="newVal">new value</param>
        /// <param name="column">for column</param>
        /// <param name="whereColumn">where this column</param>
        /// <param name="whereVal">eqauls this value</param>
        /// <returns>number of rows affected</returns>
        public int Update(string table, string newVal, string column, string whereColumn, int whereVal)
        {
            return Query("UPDATE '" + table + "' SET " + column + " = '" + newVal + "' WHERE " + whereColumn + " = " + whereVal);
        }

        /// <summary>
        /// Generic query doesn't have to be delete, helps with clarity
        /// </summary>
        /// <param name="query">query to run</param>
        /// <returns>number of rows affected</returns>
        public int Delete(string query)
        {
            return Query(query);
        }

        /// <summary>
        /// Builds a delete query
        /// </summary>
        /// <param name="table">table to delete from</param>
        /// <param name="column">column to evaluate</param>
        /// <param name="val">value to evaluate for the given column</param>
        /// <returns>number of rows affected</returns>
        public int Delete(string table, string column, string val)
        {
            return Query("DELETE FROM '" + table + "' WHERE " + column + " = '" + val + "'");
        }

        /// <summary>
        /// Builds a delete query
        /// </summary>
        /// <param name="table">table to delete from</param>
        /// <param name="column">column to evaluate</param>
        /// <param name="val">value to evaluate for the given column</param>
        /// <returns>number of rows affected</returns>
        public int Delete(string table, string column, int val)
        {
            return Query("DELETE FROM '" + table + "' WHERE " + column + " = " + val);
        }
    }
}
