using System;
using System.Collections.Generic;
using System.Text;

namespace DansWorld.Common.Net
{
    public enum PacketAction : byte
    {
        Request = 1, 
        Accept = 2, 
        Reject = 3
    }
}
