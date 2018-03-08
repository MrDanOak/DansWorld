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
        public bool IsIdle { get; private set; }
        public Direction Facing { get; private set; }
        public Character()
        {
            Name = "";
            Level = 0;
            Gender = Gender.MALE;
            IsIdle = true;
            Facing = Direction.DOWN;
        }

        public void SetFacing(Direction direction)
        {
            if ((int)direction < 4)
            {
                if (direction > 0)
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
