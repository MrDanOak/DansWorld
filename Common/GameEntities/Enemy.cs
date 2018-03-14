using DansWorld.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DansWorld.Common.GameEntities
{
    class Enemy : Character
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
    }
}
