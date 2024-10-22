﻿using DansWorld.GameClient.UI.CustomEventArgs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DansWorld.GameClient.UI
{
    /// <summary>
    /// Interface to enforce behaviour on controls
    /// </summary>
    public interface IControl
    {
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        void Update(GameTime gameTime);
        void Clicked(ClickedEventArgs e);
    }
}
