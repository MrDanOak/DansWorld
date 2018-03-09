using System;
using System.Collections.Generic;
using System.Text;
using DansWorld.Common.Enums;

namespace DansWorld.Server.GameEntities
{
    public class Character
    {
        public string Name;
        public int Level;
        public Gender Gender;
        public int ServerID;
        public int X, Y;
        public Direction Facing;
        public Character()
        {
            Name = "";
            Level = 0;
            Gender = Gender.MALE;
        }
    }
}
