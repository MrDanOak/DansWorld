using System;
using System.Collections.Generic;
using System.Text;
using DansWorld.Common.Enums;

namespace DansWorld.Common.GameEntities
{
    public class PlayerCharacter : Character
    {
        public Gender Gender;
        public PClass Class;
        public PlayerCharacter()
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
    }
}
