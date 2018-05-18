using DansWorld.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DansWorld.Common.GameEntities
{
    public class Character
    {
        public bool IsIdle;
        public bool IsWalking;
        public int X;
        public int Y;
        public int EXP;
        public string Name;
        public int Level;
        public int ID;
        public int Strength;
        public int Intelligence;
        public int Dexterity;
        public int Vitality;
        public int Health;
        public int MaxHealth;
        public Direction Facing;
    }
}
