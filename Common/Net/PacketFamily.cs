using System;
using System.Collections.Generic;
using System.Text;

namespace DansWorld.Common.Net
{
    /// <summary>
    /// Family groups of packet headers
    /// </summary>
    public enum PacketFamily : byte
    {
        LOGIN = 1, 
        REGISTER = 2, 
        PLAY = 3, 
        PLAYER = 4,
        CONNECTION = 5,
        ENEMY = 6,
        CHARACTER = 7,
        SERVER = 8
    }
}
