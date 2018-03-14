using DansWorld.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DansWorld.Common.GameEntities
{
    public class Character
    {
        public string Name;
        public int Level;
        public bool IsIdle;
        public bool IsWalking;
        public Direction Facing;
        public int EXP;
        public int X;
        public int Y;
        public int Vitality;
        public int Strength;
        public int Dexterity;
        public int Intelligence;

        public void SetFacing(Direction direction)
        {
            if ((int)direction < 4)
            {
                if (direction >= 0)
                    Facing = direction;
                else
                    Facing = (Direction)3;
            }
            else
            {
                Facing = 0;
            }
        }
    }
}
