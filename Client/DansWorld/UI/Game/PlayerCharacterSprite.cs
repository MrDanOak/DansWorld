using DansWorld.Common.GameEntities;
using DansWorld.Common.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DansWorld.GameClient.GameComponents;
using Microsoft.Xna.Framework.Content;

namespace DansWorld.GameClient.UI.Game
{
    /// <summary>
    /// Sprite container for a player
    /// </summary>
    public class PlayerCharacterSprite : CharacterSprite, IDrawable
    {
        public Rectangle SourceRectangle;
        public PlayerCharacter PlayerCharacter;
        public bool InGame = false;
        internal WeaponSprite WeaponSprite;

        private Camera2D _camera;

        private Rectangle Dest
        {
            get
            {
                return new Rectangle(Location, Size);
            }
        }

        private int _facingModifier
        {
            get
            {
                int i = 0;
                switch (PlayerCharacter.Facing)
                {
                    case Direction.DOWN: i = 0; break;
                    case Direction.LEFT: i = 12; break;
                    case Direction.RIGHT: i = 24; break;
                    case Direction.UP: i = 36; break;
                }
                return i;
            }
        }
        public PlayerCharacterSprite(ContentManager content, PlayerCharacter playerCharacter, Camera2D camera)
        {
            _camera = camera;
            _namePlate = new Label()
            {
                FrontColor = Color.Black,
                Font = GameClient.DEFAULT_FONT
            };
            PlayerCharacter = playerCharacter;
            //loading in the weapon that I crudely drew
            WeaponSprite = new WeaponSprite(playerCharacter);
            WeaponSprite.Texture = content.Load<Texture2D>("Images/Weapons/sword");
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            if (PlayerCharacter != null)
            {
                if (InGame)
                {
                    if (_mouseOver)
                    {
                        //show name plates on mouse over
                        _namePlate.Draw(gameTime, spriteBatch);
                    }
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, _camera.Transform);
                    HealthBar.Draw(gameTime, spriteBatch);
                    spriteBatch.End();
                    //attempted to do the whole depth thing here, not sure it entirely works, would need more research
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, _camera.Transform);
                    float depth = 1 - ((float)Location.Y / 10000);


                    spriteBatch.Draw(Texture,
                        new Rectangle(PlayerCharacter.X, PlayerCharacter.Y, Width, Height),
                        GetRectangleForFrameID(_animationID + (PlayerCharacter.Gender == Gender.FEMALE ? 3 : 0) + _facingModifier), //+3 to the sprite ID for females + the animation id
                        Color.White,
                        0.0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        depth);
                    //ensuring the weapons also get drawn
                    WeaponSprite.Draw(gameTime, spriteBatch);
                }
                else
                {
                    spriteBatch.Draw(Texture,
                        Destination,
                        GetRectangleForFrameID(_animationID + (PlayerCharacter.Gender == Gender.FEMALE ? 3 : 0) + _facingModifier), 
                        Color.White);
                }
            } 
        }

        public new void Update(GameTime gameTime, Camera2D camera)
        {
            base.Update(gameTime, camera);
            if (PlayerCharacter == null) return;
            if (InGame)
            {
                //updating sprite location to be that of the player character
                Location = new Point(PlayerCharacter.X, PlayerCharacter.Y);

                //putting health bars above characters
                HealthBar.Location = new Point(Location.X - 1, Location.Y - 10);
                //asigning the health of the health bar
                HealthBar.SetHP(PlayerCharacter.Health, PlayerCharacter.MaxHealth);
                
                //ensuring the two children sprites of the character sprite are updated
                HealthBar.Update(gameTime, camera);
                WeaponSprite.Update(gameTime, camera);

                if (PlayerCharacter.IsWalking) // if walking, start animation
                {
                    _animationTimer += gameTime.ElapsedGameTime.Milliseconds;
                    if (_animationTimer > 200)
                    {
                        //if 200ms has passed we're going to update the sprite by 1. if it reaches 3 loop back around to 0
                        _animationID += 1;
                        if (_animationID == 3)
                            _animationID = 0;
                        _animationTimer = 0; //reset timer
                    }
                }
                else //stand still
                {
                    _animationID = 1;
                }

                if (_mouseOver)
                {
                    if (_namePlate.Text == null || _namePlate.Text == "" && PlayerCharacter != null)
                    {
                        _namePlate.Text = PlayerCharacter.Name;
                    }
                    Vector2 dimCharName = GameClient.DEFAULT_FONT.MeasureString(PlayerCharacter.Name);
                    _namePlate.Size = new Point((int)dimCharName.X, (int)dimCharName.Y);
                    //ensuring name plates are above players and also above health bars
                    _namePlate.Location = new Point(PlayerCharacter.X + (Width / 2) - ((int)dimCharName.X / 2), PlayerCharacter.Y - (int)dimCharName.Y - HealthBar.Size.Y);
                    _namePlate.Update(gameTime);
                }
            }
            else
            {
                _animationID = 1;
            }
        }
    }
}
