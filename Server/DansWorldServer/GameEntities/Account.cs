using DansWorld.Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DansWorld.Server.GameEntities
{

    public enum AccountState
    {
        LoggedOut,
        LoggedIn, 
        Playing
    }
    public class Account
    {
        public string Username = "";
        public string Password = "";
        public string Email = "";
        public List<Character> Characters;
        public AccountState State = AccountState.LoggedOut;

        public string Fullname { get; private set; }

        public Account(string username, string password, string email, string fullname)
        {
            Username = username;
            Password = password;
            Characters = new List<Character>();
            Fullname = fullname;
        }

        public Character GetCharacter(string name)
        {
            foreach (Character character in Characters)
            {
                if (character.Name == name)
                    return character;
            }
            return null;
        }

        public Character GetCharacter(int id)
        {
            if (id < Characters.Count)
                return Characters[id];
            else
                return null;
        }
        
        public void CreateDatabaseEntry(Database database, string ip)
        {
            string query = "INSERT INTO Accounts (Username, Email, Password, Fullname, RegisteredIP, LastUsedIP, Created)" 
                + " VALUES ('" + Username + "','" + Email + "','" + Password + "','" + Fullname + "','" + ip + "', '" + ip + "', '" 
                + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');";
            database.Insert(query);
        }
    }
}
