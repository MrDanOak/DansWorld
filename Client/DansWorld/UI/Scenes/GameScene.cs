using DansWorld.Common.GameEntities;
using DansWorld.Common.Net;
using DansWorld.GameClient.GameComponents;
using DansWorld.GameClient.UI.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace DansWorld.GameClient.UI.Scenes
{
    public class GameScene : BaseScene
    {
        private List<PlayerCharacterSprite> _characterSprites;
        private List<EnemySprite> _enemySprites;
        private ContentManager _content;
        private GameClient _gameClient;
        private Label _pingLabel;
        private List<Label> _lblMessages;
        private TextBox _txtIn;
        private bool _serverNotifiedOfIdle = false;
        private bool _enterDown = false;
        private int attackTimer = 0;
        private Camera2D _camera;

        private PlayerCharacterSprite _thisPlayer
        {
            get
            {
                return (_characterSprites != null && _characterSprites.Count > 0 ? _characterSprites[0] : null);
            }
        }


        public List<PlayerCharacter> PlayerCharacters
        {
            get
            {
                List<PlayerCharacter> ret = new List<PlayerCharacter>();
                foreach (PlayerCharacterSprite sprite in _characterSprites)
                {
                    ret.Add(sprite.PlayerCharacter);
                }
                return ret;
            }
        }

        public List<Enemy> Enemies
        {
            get
            {
                List<Enemy> ret = new List<Enemy>();
                foreach (EnemySprite sprite in _enemySprites)
                {
                    ret.Add(sprite.Enemy);
                }
                return ret;
            }
        }

        public GameScene(GameClient gameClient)
        {
            Controls = new List<Control>();
            _lblMessages = new List<Label>();
            _characterSprites = new List<PlayerCharacterSprite>();
            _enemySprites = new List<EnemySprite>();
            _gameClient = gameClient;
            _gameClient.Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            if(_txtIn != null) _txtIn.Location = new Point(10, GameClient.HEIGHT - (int)GameClient.DEFAULT_FONT.MeasureString(" ").Y - 5);
            for (int i = 0; i < 10; i++)
            {
                if (_lblMessages[i] != null)
                {
                    _lblMessages[i].Location = new Point(10,
                        GameClient.HEIGHT - ((i + 1) * (int)GameClient.DEFAULT_FONT.MeasureString("test").Y) - _txtIn.Destination.Height - 5);
                }
            }
        }

        public override void Initialise(ContentManager Content)
        {
            _camera = new Camera2D(_gameClient);
            _camera.Initialize();
            _content = Content;
            _pingLabel = new Label()
            {
                Font = GameClient.DEFAULT_FONT,
                FrontColor = Color.Black,
                Location = new Point(0, 0),
                Text = "0 ms",
            };

            _txtIn = new TextBox()
            {
                FrontColor = Color.Black,
                BackColor = Color.White,
                BorderColor = Color.Black,
                Font = GameClient.DEFAULT_FONT,
                NumbersAllowed = true,
                SpecialCharactersAllowed = true,
                IsVisible = true,
                SpacesAllowed = true,
                CharacterLimit = 300,
                Name = "txtIn",
                Location = new Point(10, GameClient.HEIGHT - (int)GameClient.DEFAULT_FONT.MeasureString(" ").Y - 5),
                Size = new Point(300, (int)GameClient.DEFAULT_FONT.MeasureString(" ").Y)
            };

            for (int i = 0; i < 10; i++)
            {
                _lblMessages.Add(new Label()
                {
                    Font = GameClient.DEFAULT_FONT_BOLD,
                    FrontColor = Color.Black,
                    IsVisible = true,
                    Location = new Point(10, GameClient.HEIGHT - ((i + 1) * (int)GameClient.DEFAULT_FONT.MeasureString("test").Y) - _txtIn.Destination.Height - 5)
                });
            }
        }

        public void AddCharacter(PlayerCharacter player)
        {
            PlayerCharacterSprite sprite = new PlayerCharacterSprite(_content, player, _camera)
            {
                IsVisible = true,
                Texture = _content.Load<Texture2D>("Images/Characters/base"),
                Width = 48,
                Height = 48,
                Size = new Point(48, 48),
                Location = new Point(player.X, player.Y),
                PlayerCharacter = player,
                InGame = true,
            };
            sprite.HealthBar.SetHP(player.Health, player.MaxHealth);
            _characterSprites.Add(sprite);
        }

        public void RemoveCharacter(PlayerCharacter player)
        {
            PlayerCharacterSprite toRemove = new PlayerCharacterSprite(_content, player, _camera);
            foreach (PlayerCharacterSprite sprite in _characterSprites)
            {
                if (player == sprite.PlayerCharacter)
                {
                    toRemove = sprite;
                }
            }
            _characterSprites.Remove(toRemove);
        }

        internal void AddEnemy(Enemy enemy)
        {
            EnemySprite enemySprite = new EnemySprite()
            {
                IsVisible = true,
                Texture = _content.Load<Texture2D>("Images/Characters/enemies"),
                Width = 48,
                Height = 48,
                Size = new Point(48, 48),
                Location = new Point(enemy.X, enemy.Y), 
                Enemy = enemy
            };
            _enemySprites.Add(enemySprite);
        }

        public void ClearCharacters()
        {
            _characterSprites = new List<PlayerCharacterSprite>();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _pingLabel.Draw(gameTime, spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, _camera.Transform);
            foreach (PlayerCharacterSprite characterSprite in _characterSprites)
            {
                characterSprite.Draw(gameTime, spriteBatch);
            }
            foreach (EnemySprite enemySprite in _enemySprites)
            {
                enemySprite.Draw(gameTime, spriteBatch, _camera);
            }
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            null, null, null, null);
            foreach (Label lbl in _lblMessages)
            {
                lbl.Draw(gameTime, spriteBatch);
            }
            _txtIn.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            _pingLabel.Update(gameTime);
            _camera.Update(gameTime);
            if (_characterSprites.Count > 0 && _camera.Focus != _characterSprites[0]) 
                _camera.Focus = _characterSprites[0];

            _thisPlayer.PlayerCharacter.IsAttacking = false;

            bool moved = false;

            if (!_txtIn.HasFocus)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.A)) { _characterSprites[0].PlayerCharacter.X -= 1; _characterSprites[0].PlayerCharacter.Facing = Common.Enums.Direction.LEFT; moved = true; }
                else if (Keyboard.GetState().IsKeyDown(Keys.S)) { _characterSprites[0].PlayerCharacter.Y += 1; _characterSprites[0].PlayerCharacter.Facing = Common.Enums.Direction.DOWN; moved = true; }
                else if (Keyboard.GetState().IsKeyDown(Keys.D)) { _characterSprites[0].PlayerCharacter.X += 1; _characterSprites[0].PlayerCharacter.Facing = Common.Enums.Direction.RIGHT; moved = true; }
                else if (Keyboard.GetState().IsKeyDown(Keys.W)) { _characterSprites[0].PlayerCharacter.Y -= 1; _characterSprites[0].PlayerCharacter.Facing = Common.Enums.Direction.UP; moved = true; }
                else if (Keyboard.GetState().IsKeyUp(Keys.Enter) && _enterDown) { _txtIn.HasFocus = true; }


                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    attackTimer += gameTime.ElapsedGameTime.Milliseconds;
                    if (attackTimer > 500)
                    {
                        PacketBuilder pb = new PacketBuilder(PacketFamily.PLAYER, PacketAction.ATTACK);
                        pb.AddInt(_gameClient.CharacterID);
                        GameClient.NetClient.Send(pb.Build());
                        attackTimer = 0;
                        _thisPlayer.PlayerCharacter.IsAttacking = true;
                    }
                }
                else
                {
                    attackTimer = 0;
                }

                // This is in place to stop server spam, otherwise every time the sprite is updated
                // it will send the server the characters x and y (Many times a second)
                if (moved)
                {
                    PacketBuilder pb = new PacketBuilder(PacketFamily.PLAYER, PacketAction.MOVE);
                    pb = new PacketBuilder(PacketFamily.PLAYER, PacketAction.MOVE)
                        .AddInt(_characterSprites[0].PlayerCharacter.X)
                        .AddInt(_characterSprites[0].PlayerCharacter.Y)
                        .AddByte((byte)_characterSprites[0].PlayerCharacter.Facing)
                        .AddInt(_gameClient.CharacterID);
                    GameClient.NetClient.Send(pb.Build());

                    _serverNotifiedOfIdle = false;
                }
                else if (!_serverNotifiedOfIdle)
                {
                    PacketBuilder pb = new PacketBuilder(PacketFamily.PLAYER, PacketAction.STOP)
                        .AddInt(_characterSprites[0].PlayerCharacter.X)
                        .AddInt(_characterSprites[0].PlayerCharacter.Y)
                        .AddByte((byte)_characterSprites[0].PlayerCharacter.Facing)
                        .AddInt(_gameClient.CharacterID);
                    GameClient.NetClient.Send(pb.Build());
                    _serverNotifiedOfIdle = true;
                }
            }
            else
            {
                if (Keyboard.GetState().IsKeyUp(Keys.Enter) && _enterDown)
                {
                    if (_txtIn.Text != "")
                    {
                        PacketBuilder pb = new PacketBuilder(PacketFamily.PLAYER, PacketAction.TALK);
                        pb = pb.AddInt(_txtIn.Text.Length)
                            .AddString(_txtIn.Text)
                            .AddInt(_gameClient.CharacterID);
                        GameClient.NetClient.Send(pb.Build());
                        if (_txtIn.Text.Split(' ')[0] == "SetHP")
                            _characterSprites[0].PlayerCharacter.Health = Convert.ToInt32(_txtIn.Text.Split(' ')[1]);
                        _txtIn.Text = "";
                    }
                    _txtIn.HasFocus = false;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    _txtIn.HasFocus = false;
                }
            }

            _characterSprites[0].PlayerCharacter.IsIdle = !moved;
            _characterSprites[0].PlayerCharacter.IsWalking = moved;

            foreach (PlayerCharacterSprite characterSprite in _characterSprites)
            {
                characterSprite.Update(gameTime, _camera);
            }

            foreach (EnemySprite enemySprite in _enemySprites)
            {
                enemySprite.Update(gameTime, _camera);
            }

            foreach (Label lbl in _lblMessages)
            {
                lbl.Update(gameTime);
            }
            _enterDown = Keyboard.GetState().IsKeyDown(Keys.Enter);
            _txtIn.Update(gameTime);
            base.Update(gameTime);
        }

        internal void ShowPing(int ms)
        {
            _pingLabel.Text = ms + " ms";
            _pingLabel.Size = new Point(
                (int)_pingLabel.Font.MeasureString(_pingLabel.Text).X,
                (int)_pingLabel.Font.MeasureString(_pingLabel.Text).Y);

        }

        internal void DisplayMessage(string message, string from)
        {
            for (int i = _lblMessages.Count - 1; i > 0; i--)
            {
                _lblMessages[i].Text = _lblMessages[i - 1].Text;
                _lblMessages[i].Size = new Point((int)_lblMessages[i].Font.MeasureString(_lblMessages[i].Text).X, 
                                                 (int)_lblMessages[i].Font.MeasureString(_lblMessages[i].Text).Y);
                if (_lblMessages[i].Text == "") _lblMessages[i].IsVisible = false;
                else _lblMessages[i].IsVisible = true;
            }
            _lblMessages[0].Text = "[" + from + "] " + message;
            _lblMessages[0].Size = new Point((int)_lblMessages[0].Font.MeasureString(_lblMessages[0].Text).X,
                                                 (int)_lblMessages[0].Font.MeasureString(_lblMessages[0].Text).Y);
        }
    }
}
