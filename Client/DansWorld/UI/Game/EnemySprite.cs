using DansWorld.Common.Enums;
using DansWorld.Common.GameEntities;
using DansWorld.GameClient.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DansWorld.GameClient.UI.Game
{
    class EnemySprite : CharacterSprite, IDrawable
    {
        public Enemy Enemy;

        public override void Update(GameTime gameTime, Camera2D camera)
        {
            base.Update(gameTime, camera);
            _animationTimer += gameTime.ElapsedGameTime.Milliseconds;
            Location = new Point(Enemy.X, Enemy.Y);

            HealthBar.Location = new Point(Location.X - 1, Location.Y - 10);
            HealthBar.SetHP(Enemy.Health, Enemy.MaxHealth);
            HealthBar.Update(gameTime, camera);

            if (_animationTimer > 400)
            {
                _animationID += 1;
                if (_animationID == 3) _animationID = 0;
                _animationTimer = 0;
            }

            if (_mouseOver)
            {
                if (_namePlate.Text == null || _namePlate.Text == "" && Enemy != null)
                {
                    _namePlate.Text = Enemy.Name;
                }
                Vector2 dimCharName = GameClient.DEFAULT_FONT.MeasureString(Enemy.Name);
                _namePlate.Size = new Point((int)dimCharName.X, (int)dimCharName.Y);
                _namePlate.Location = new Point(Enemy.X + (Width / 2) - ((int)dimCharName.X / 2), Enemy.Y - (int)dimCharName.Y);
                _namePlate.Update(gameTime);
            }
        }

        public EnemySprite()
        {
            _namePlate = new Label()
            {
                FrontColor = Color.Black,
                Font = GameClient.DEFAULT_FONT
            };
        }

        private int _facingModifier
        {
            get
            {
                int i = 0;
                switch (Enemy.Facing)
                {
                    case Direction.DOWN: i = 0; break;
                    case Direction.LEFT: i = 12; break;
                    case Direction.RIGHT: i = 24; break;
                    case Direction.UP: i = 36; break;
                }
                return i;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera2D camera)
        {
            float depth = 1 - ((float)Location.Y / GameClient.HEIGHT);

            spriteBatch.Draw(Texture,
                new Rectangle(Enemy.X, Enemy.Y, Width, Height),
                GetRectangleForFrameID(Enemy.SpriteID  + _animationID + _facingModifier),
                Color.White,
                0.0f,
                Vector2.Zero,
                SpriteEffects.None,
                depth);


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.Transform);
            HealthBar.Draw(gameTime, spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.Transform);

            if (_mouseOver)
            {
                _namePlate.Draw(gameTime, spriteBatch);
            }
        }
    }
}
