using System;
using System.Collections.Generic;
using System.Text;
using DansWorld.Common.Enums;
using DansWorld.Common.IO;
using DansWorld.Server.Data;

namespace DansWorld.Server.GameEntities
{
    public class PlayerCharacter : Character
    {
        /// <summary>
        /// Gender of the character
        /// </summary>
        public Gender Gender;
        /// <summary>
        /// ID given to the character by the server
        /// </summary>
        public int ServerID;
        public PlayerCharacter()
        {
            Name = "";
            Level = 0;
            Gender = Gender.MALE;
        }

        /// <summary>
        /// Creates a character entry into the database
        /// </summary>
        /// <param name="database">database object to use</param>
        /// <param name="owner">the owner of the account</param>
        public void CreateDatabaseEntry(Database database, string owner)
        {
            Logger.Log($"Character creation requested name {Name} account {owner}");
            string query = $"INSERT INTO Characters (CharacterName, AccountUsername, EXP, Facing, HP, Level, Map, Standing, X, Y, Strength, Dexterity, Vitality, Intelligence," +
                $" Class, Gender) VALUES ('{Name}', '{owner}', {EXP}, {(byte)Facing}, {Health}, {Level}, 1, 1, {X}, {Y}, {Strength}, {Dexterity}, {Vitality}, {Intelligence}, 0, {(byte)Gender}); ";
            database.Insert(query);
        }

        /// <summary>
        /// Saves the state of a character
        /// </summary>
        /// <param name="database">database object</param>
        public void Save(Database database)
        {
            Logger.Log("Saving Character: " + Name + " information");
            string query = "UPDATE Characters SET EXP = " + EXP + ", Level = " + Level + ", X = " + X + ", Y = " + Y + ", Facing = " 
                           + (int)Facing + " WHERE CharacterName = '" + Name + "'";
            database.Update(query);
        }
    }
}
