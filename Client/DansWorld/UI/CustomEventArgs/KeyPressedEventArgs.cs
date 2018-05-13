using System;
using Microsoft.Xna.Framework.Input;

namespace DansWorld.GameClient.UI.CustomEventArgs
{
    class KeyPressedEventArgs : EventArgs
    {
        public Keys KeyPressed;
        public KeyPressedEventArgs(Keys keyPressed)
        {
            KeyPressed = keyPressed;
        }
    }
}
