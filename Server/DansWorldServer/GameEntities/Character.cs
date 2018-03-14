using System;
using System.Collections.Generic;
using System.Text;
using DansWorld.Common.Enums;
using DansWorld.Common.IO;
using DansWorld.Server.Data;

namespace DansWorld.Server.GameEntities
{
    public class Character
    {
        public string Name;
        public int Level;
        public Gender Gender;
        public int ServerID;
        public int X, Y, EXP;
        public Direction Facing;
        public Character()
        {
            Name = "";
            Level = 0;
            Gender = Gender.MALE;
        }

        public void Save(Database database)
        {
            Logger.Log("Saving Character: " + Name + " information");
            string query = "UPDATE Characters SET EXP = " + EXP + ", Level = " + Level + ", X = " + X + ", Y = " + Y + ", Facing = " 
                           + (int)Facing + " WHERE CharacterName = '" + Name + "'";
            database.Update(query);
        }
    }
}
