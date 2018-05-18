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
        public List<PlayerCharacter> Contributors = new List<PlayerCharacter>();
        public int SpawnX;
        public int SpawnY;
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

        /// <summary>
        /// default enemy constructor
        /// </summary>
        public Enemy()
        {
        }


        /// <summary>
        /// Used to move the enemy randomly
        /// </summary>
        /// <returns>true if the enemy moved, false if not</returns>
        public bool IdleMove()
        {
            //we want to move 10 times in the given direction
            if (_moveCount < 10)
            {
                switch (Facing)
                {
                    case Direction.DOWN: Y += 1; break;
                    case Direction.LEFT: X -= 1; break;
                    case Direction.RIGHT: X += 1; break;
                    case Direction.UP: Y -= 1; break;
                }

                //keeping the enemies on grid
                if (X < 0) X = 0;
                if (Y < 0) Y = 0;
                _moveCount += 1;
            }
            else
            {
                int rnd = RNG.Next(0, 100);
                //1% chance to start moving if its not moving 99% chance to change direction if it is moving
                if (rnd <= 1 && !_isMoving || rnd < 99 && _isMoving)
                {
                    rnd = RNG.Next(0, 100);
                    if (rnd < 99)
                    {
                        //99% chance to change direction
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
