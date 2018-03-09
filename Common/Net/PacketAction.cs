using System;
using System.Collections.Generic;
using System.Text;

namespace DansWorld.Common.Net
{
    public enum PacketAction : byte
    {
        REQUEST = 1, 
        ACCEPT = 2, 
        REJECT = 3, 
        MOVE = 4, 
        STOP = 5, 
        WELCOME = 6,
        LOGOUT = 7
    }
}
