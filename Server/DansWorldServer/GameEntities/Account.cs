using DansWorld.Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DansWorld.Server.GameEntities
{
    /// <summary>
    /// State of the account
    /// </summary>
    public enum AccountState
    {
        LoggedOut,
        LoggedIn, 
        Playing
    }
    public class Account
    {
        /// <summary>
        /// Username of the account
        /// </summary>
        public string Username = "";
        /// <summary>
        /// Password of the account
        /// </summary>
        public string Password = "";
        /// <summary>
        /// Email of the account
        /// </summary>
        public string Email = "";
        /// <summary>
        /// Collection of characters belonging to the account
        /// </summary>
        public List<PlayerCharacter> Characters;
        /// <summary>
        /// State of the account
        /// </summary>
        public AccountState State = AccountState.LoggedOut;
        /// <summary>
        /// Full name of the owner of the account
        /// </summary>
        public string Fullname { get; private set; }
        /// <summary>
        /// Constructor of an account
        /// </summary>
        /// <param name="username">Usernmae of the account</param>
        /// <param name="password">Hashed password of the account</param>
        /// <param name="email">email of the account</param>
        /// <param name="fullname">full name of the account owner</param>
        public Account(string username, string password, string email, string fullname)
        {
            Username = username;
            Password = password;
            Characters = new List<PlayerCharacter>();
            Fullname = fullname;
        }

        /// <summary>
        /// Helper method used to get a character of the account by its name
        /// </summary>
        /// <param name="name">name of the character to search for</param>
        /// <returns>returns a character object with the name matching that given, if not, returns null</returns>
        public PlayerCharacter GetCharacter(string name)
        {
            foreach (PlayerCharacter character in Characters)
            {
                //if the character name matches the name given to the function, return that character
                if (character.Name == name)
                    return character;
            }
            return null;
        }

        /// <summary>
        /// Helper method to get a character from an account by its id
        /// </summary>
        /// <param name="id">id of the character to find</param>
        /// <returns>the found character, if not, returns null</returns>
        public PlayerCharacter GetCharacter(int id)
        {
            if (id < Characters.Count)
                return Characters[id];
            else
                return null;
        }
        
        /// <summary>
        /// stores a newly made character into the database
        /// </summary>
        /// <param name="database">database object</param>
        /// <param name="ip">ip of the connection when the account was created</param>
        public void CreateDatabaseEntry(Database database, string ip)
        {
            string query = "INSERT INTO Accounts (Username, Email, Password, Fullname, RegisteredIP, LastUsedIP, Created)" 
                + " VALUES ('" + Username + "','" + Email + "','" + Password + "','" + Fullname + "','" + ip + "', '" + ip + "', '" 
                + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');";
            database.Insert(query);
        }
    }
}
