using DansWorld.Common.GameEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DansWorld.Common.Enums;
using DansWorld.GameClient.GameComponents;

namespace DansWorld.GameClient.UI.Game
{
    class WeaponSprite : BaseGameSprite, IDrawable
    {
        PlayerCharacter _playerCharacter;
        float rotation = 0;
        public WeaponSprite(PlayerCharacter playerCharacter)
        {
            _playerCharacter = playerCharacter;
            IsVisible = true;
            Size = new Point(48, 48);
            Width = 48;
            Height = 48;
        }
        public virtual void Update(GameTime gameTime, Camera2D camera)
        {
            base.Update(gameTime, camera);

            if (_playerCharacter != null)
            {
                switch (_playerCharacter.Facing)
                {
                    case Direction.LEFT:
                        rotation = 3.1f;
                        Location = new Point(_playerCharacter.X + 10, _playerCharacter.Y + 24);
                        break;
                    case Direction.DOWN:
                        rotation = 1.55f;
                        Location = new Point(_playerCharacter.X + 24, _playerCharacter.Y + 40);
                        break;
                    case Direction.RIGHT:
                        rotation = 0;
                        Location = new Point(_playerCharacter.X + 38, _playerCharacter.Y + 24);
                        break;
                    case Direction.UP:
                        rotation = 4.65f;
                        Location = new Point(_playerCharacter.X + 24, _playerCharacter.Y + 8);
                        break;
                }
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_playerCharacter != null)
            {
                spriteBatch.Draw(Texture,
                        new Rectangle(Location.X, Location.Y, Width, Height),
                        GetRectangleForFrameID(0),
                        Color.White,
                        (float)rotation,
                        new Vector2(0,24),
                        SpriteEffects.None,
                        0);
            }
        }
    }
}
