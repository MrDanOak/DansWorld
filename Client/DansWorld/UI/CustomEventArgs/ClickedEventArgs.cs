using Microsoft.Xna.Framework;
using System;

namespace DansWorld.GameClient.UI.CustomEventArgs
{
    /// <summary>
    /// custom event for storing mouse clicks
    /// </summary>
    public class ClickedEventArgs : EventArgs
    {
        public Point Location;
        public ClickedEventArgs(Point location)
        {
            Location = location;
        }
    }
}
