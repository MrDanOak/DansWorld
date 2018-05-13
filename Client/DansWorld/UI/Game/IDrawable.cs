using DansWorld.GameClient.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansWorld.GameClient.UI.Game
{
    public interface IDrawable
    {
        void Update(GameTime gameTime, Camera2D camera);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
