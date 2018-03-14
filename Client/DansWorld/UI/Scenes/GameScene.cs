using DansWorld.Common.GameEntities;
using DansWorld.Common.Net;
using DansWorld.GameClient.UI.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansWorld.GameClient.UI.Scenes
{
    class GameScene : BaseScene
    {
        private List<CharacterSprite> characterSprites;
        private ContentManager _content;
        private GameClient _gameClient;
        private Label _pingLabel;
        private List<Label> _lblMessages;
        private TextBox _txtIn;
        private bool _serverNotifiedOfIdle = false;
        private bool _enterDown = false;


        public List<Character> Characters
        {
            get
            {
                List<Character> ret = new List<Character>();
                foreach (CharacterSprite sprite in characterSprites)
                {
                    ret.Add(sprite.Character);
                }
                return ret;
            }
        }

        public GameScene(GameClient gameClient)
        {
            Controls = new List<Control>();
            _lblMessages = new List<Label>();
            _gameClient = gameClient;
        }

        public override void Initialise(ContentManager Content)
        {
            characterSprites = new List<CharacterSprite>();
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
                    Font = GameClient.DEFAULT_FONT,
                    FrontColor = Color.Black,
                    IsVisible = true,
                    Location = new Point(10, GameClient.HEIGHT - ((i + 1) * (int)GameClient.DEFAULT_FONT.MeasureString("test").Y) - _txtIn.Destination.Height - 5)
                });
            }
        }

        public void AddCharacter(Character character)
        {
            CharacterSprite sprite = new CharacterSprite()
            {
                Name = character.Name + "sprite",
                IsVisible = true,
                BaseTexture = _content.Load<Texture2D>("Images/Characters/base"),
                SpriteWidth = 48,
                SpriteHeight = 48,
                Size = new Point(48, 48),
                Location = new Point(character.X, character.Y),
                FrontColor = Color.White,
                Character = character,
                InGame = true
            };
            characterSprites.Add(sprite);
        }

        public void RemoveCharacter(Character character)
        {
            CharacterSprite toRemove = new CharacterSprite();
            foreach (CharacterSprite sprite in characterSprites)
            {
                if (character == sprite.Character)
                {
                    toRemove = sprite;
                }
            }
            characterSprites.Remove(toRemove);
        }

        public void ClearCharacters()
        {
            characterSprites = new List<CharacterSprite>();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // TODO: 
            // Sort character sprites by what is in front first
            _pingLabel.Draw(gameTime, spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.BackToFront);
            foreach (CharacterSprite characterSprite in characterSprites)
            {
                characterSprite.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();

            spriteBatch.Begin();
            foreach (Label lbl in _lblMessages)
            {
                lbl.Draw(gameTime, spriteBatch);
            }
            _txtIn.Draw(gameTime, spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            _pingLabel.Update(gameTime);
            bool moved = false;

            if (!_txtIn.HasFocus)
            {
                PacketBuilder pb = new PacketBuilder(PacketFamily.PLAYER, PacketAction.MOVE);
                if (Keyboard.GetState().IsKeyDown(Keys.A)) { characterSprites[0].Character.X -= 1; characterSprites[0].Character.SetFacing(Common.Enums.Direction.LEFT); moved = true; }
                else if (Keyboard.GetState().IsKeyDown(Keys.S)) { characterSprites[0].Character.Y += 1; characterSprites[0].Character.SetFacing(Common.Enums.Direction.DOWN); moved = true; }
                else if (Keyboard.GetState().IsKeyDown(Keys.D)) { characterSprites[0].Character.X += 1; characterSprites[0].Character.SetFacing(Common.Enums.Direction.RIGHT); moved = true; }
                else if (Keyboard.GetState().IsKeyDown(Keys.W)) { characterSprites[0].Character.Y -= 1; characterSprites[0].Character.SetFacing(Common.Enums.Direction.UP); moved = true; }
                else if (Keyboard.GetState().IsKeyUp(Keys.Enter) && _enterDown) { _txtIn.HasFocus = true; }

                // This is in place to stop server spam, otherwise every time the sprite is updated
                // it will send the server the characters x and y (Many times a second)
                if (moved)
                {
                    pb = new PacketBuilder(PacketFamily.PLAYER, PacketAction.MOVE)
                        .AddInt(characterSprites[0].Character.X)
                        .AddInt(characterSprites[0].Character.Y)
                        .AddByte((byte)characterSprites[0].Character.Facing)
                        .AddInt(_gameClient.CharacterID);
                    GameClient.NetClient.Send(pb.Build());

                    _serverNotifiedOfIdle = false;
                }
                else if (!_serverNotifiedOfIdle)
                {
                    pb = new PacketBuilder(PacketFamily.PLAYER, PacketAction.STOP)
                        .AddInt(characterSprites[0].Character.X)
                        .AddInt(characterSprites[0].Character.Y)
                        .AddByte((byte)characterSprites[0].Character.Facing)
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
                        _txtIn.Text = "";
                    }
                    _txtIn.HasFocus = false;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    _txtIn.HasFocus = false;
                }
            }

            characterSprites[0].Character.IsIdle = !moved;
            characterSprites[0].Character.IsWalking = moved;

            foreach (CharacterSprite characterSprite in characterSprites)
            {
                characterSprite.Update(gameTime);
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

        internal void ShowMessage(string message, string from)
        {
            for (int i = _lblMessages.Count - 1; i > 0; i--)
            {
                _lblMessages[i].Text = _lblMessages[i - 1].Text;
                _lblMessages[i].Size = new Point((int)_lblMessages[i].Font.MeasureString(_lblMessages[i].Text).X, 
                                                 (int)_lblMessages[i].Font.MeasureString(_lblMessages[i].Text).Y);
                if (_lblMessages[i].Text == "") _lblMessages[i].IsVisible = false;
                else _lblMessages[i].IsVisible = true;
            }
            _lblMessages[0].Text = from + ": " + message;
            _lblMessages[0].Size = new Point((int)_lblMessages[0].Font.MeasureString(_lblMessages[0].Text).X,
                                                 (int)_lblMessages[0].Font.MeasureString(_lblMessages[0].Text).Y);
        }
    }
}
