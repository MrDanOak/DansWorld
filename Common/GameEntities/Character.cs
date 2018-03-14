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
        public bool IsIdle { get; set; }
        public bool IsWalking { get; set; }
        public Direction Facing { get; set; }

        public int X, Y, ServerID;
        public Character()
        {
            Name = "";
            Level = 0;
            Gender = Gender.MALE;
            IsIdle = true;
            IsWalking = false;
            Facing = Direction.DOWN;
            X = 0;
            Y = 0;
        }

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
