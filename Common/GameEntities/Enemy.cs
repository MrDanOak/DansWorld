using DansWorld.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DansWorld.Common.GameEntities
{
    public class Enemy : Character
    {
        public int EXPReward
        {
            get
            {
                return EXP;
            }
            set
            {
                EXP = value;
            }
        }

        public int SpriteID;

        public int ServerID;

        public Enemy()
        {

        }
    }
}
