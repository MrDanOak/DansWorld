using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace DansWorld.UI.CustomEventArgs
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
