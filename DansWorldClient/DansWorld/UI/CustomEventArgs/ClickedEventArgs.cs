using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansWorld.UI.CustomEventArgs
{
    public class ClickedEventArgs : EventArgs
    {
        public Point Location;
        public ClickedEventArgs(Point location)
        {
            Location = location;
        }
    }
}
