using System;
using System.Collections.Generic;
using System.Text;
using DansWorld.Common.Enums;

namespace DansWorld.Common.GameEntities
{
    public class Character
    {
        public string Name;
        public int Level;
        public Gender Gender;
        public bool IsIdle = true;
        public Character()
        {
            Name = "";
            Level = 0;
            Gender = Gender.MALE;
        }
    }
}
