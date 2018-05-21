using System;
using Microsoft.Xna.Framework.Input;

namespace DansWorld.GameClient.UI.CustomEventArgs
{
    /// <summary>
    /// custom event for storing key presses
    /// </summary>
    class KeyPressedEventArgs : EventArgs
    {
        public Keys KeyPressed;
        public KeyPressedEventArgs(Keys keyPressed)
        {
            KeyPressed = keyPressed;
        }
    }
}
