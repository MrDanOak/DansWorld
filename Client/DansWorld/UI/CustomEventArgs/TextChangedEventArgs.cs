using System;

namespace DansWorld.GameClient.UI.CustomEventArgs
{
    /// <summary>
    /// Custom event for storing text changed data
    /// </summary>
    class TextChangedEventArgs : EventArgs
    {
        public string Text;
        public string OldText;
        public TextChangedEventArgs(string text, string oldText)
        {
            Text = text;
            OldText = oldText;
        }
    }
}
