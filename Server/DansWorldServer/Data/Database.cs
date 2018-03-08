using DansWorld.Common.Enums;
using DansWorld.Common.IO;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DansWorld.Server.Data
{
    class Database
    {
        private MySqlConnection Connection { get; set; }
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
            Connection = new MySqlConnection(ConnectionString);
        }

        private bool openConnection()
        {
            try
            {
                Connection.Open();
                return true;
            }
            catch (MySqlException e)
            {

                switch (e.Number)
                {
                    case 0:
                        Logger.Error("Cannot connect to database. Contact administrator.");
                        break;
                    case 1045:
                        Logger.Error("Invalid username/password.");
                        break;
                }
                Logger.Error(e.Message);
                return false;
            }
        }

        public DataTable Select(string what, string from, string col = "", string whereVal = "")
        {
            openConnection();
            MySqlCommand Command = new MySqlCommand("SELECT " + what + " FROM " + from + (col != "" ? " WHERE " + col + " = '" + whereVal + "'" : ""), Connection);
            DataTable dt = new DataTable();
            MySqlDataReader reader = Command.ExecuteReader();
            dt.Load(reader);
            reader.Close();
            closeConnection();
            return dt;
        }

        private bool closeConnection()
        {
            try
            {
                Connection.Close();
                return true;
            }
            catch (MySqlException e)
            {
                Logger.Log(e.Message, LogLevel.ERROR);
                return false;
            }
        }

        public int Insert(string query)
        {
            return Query(query);
        }

        public int Query(string query)
        {
            if (openConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, Connection);
                int returnRows = cmd.ExecuteNonQuery();
                closeConnection();
                return returnRows;
            }
            return -1;
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

        public int Count(string query)
        {
            int count = -1;
            if (openConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, Connection);
                count = int.Parse(cmd.ExecuteScalar() + "");
                closeConnection();
                return count;
            }
            else
            {
                return count;
            }
        }
    }
}
