using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DansWorld.GameClient.UI.CustomEventArgs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DansWorld.GameClient.UI
{
    class Image : Control
    {
        private Texture2D _texture;
        private Rectangle? _source;
        private float _rotation;
        private Vector2 _origin;
        private SpriteEffects _effects;
        public Image(Texture2D texture)
        {
            _texture = texture;
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Destination, _source, BackColor, _rotation, _origin, _effects, 0.0f);
        }
        public override void Update(GameTime gameTime)
        {

        }
    }
}
