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

        public Account(string username, string password, string email)
        {
            Username = username;
            Password = password;
            Characters = new List<Character>();
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
    }
}
