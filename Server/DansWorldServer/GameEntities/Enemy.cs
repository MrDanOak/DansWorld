using DansWorld.Common.Enums;
using DansWorld.Server.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace DansWorld.Server.GameEntities
{
    public class Enemy : Character
    {
        public int SpriteID { get; set; }
        private bool _isMoving = false;
        private int _moveCount = 0;
        public int EXPReward
        {
            get
            {
                return EXP;
            }
            set
            {
                EXP = EXPReward;
            }
        }
        public Enemy(string name, int x, int y, int level, int strength, int vitality, int exp, int spriteID)
        {
            Name = name;
            X = x;
            Y = y;
            Level = level;
            Strength = strength;
            EXPReward = exp;
            Facing = Direction.DOWN;
            SpriteID = spriteID;
        }

        public Enemy(string name, int level, int vitality, int strength, int exp, int id)
        {
            Name = name;
            Level = level;
            Vitality = vitality;
            Strength = strength;
            EXP = exp;
            Facing = Direction.DOWN;
            ID = id;
        }

        public Enemy()
        {
        }

        public bool IdleMove()
        {
            if (_moveCount < 10)
            {
                switch (Facing)
                {
                    case Direction.DOWN: Y += 1; break;
                    case Direction.LEFT: X -= 1; break;
                    case Direction.RIGHT: X += 1; break;
                    case Direction.UP: Y -= 1; break;
                }

                if (X < 0) X = 0;
                if (Y < 0) Y = 0;
                _moveCount += 1;
            }
            else
            {
                int rnd = RNG.Next(0, 100);
                if (rnd <= 1 && !_isMoving || rnd < 99 && _isMoving)
                {
                    rnd = RNG.Next(0, 100);
                    if (rnd < 99)
                    {
                        rnd = RNG.Next(0, 4);
                        Facing = (Direction)rnd;
                    }
                    _isMoving = true;
                    _moveCount = 0;
                }
                else
                {
                    _isMoving = false;
                }
            }

            return _isMoving;
        }
    }
}
