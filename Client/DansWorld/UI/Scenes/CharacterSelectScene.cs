using System;
using System.Collections.Generic;
using DansWorld.Common.GameEntities;
using DansWorld.Common.Net;
using DansWorld.GameClient.UI.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DansWorld.GameClient.UI.Scenes
{
    public class CharacterSelectScene : BaseScene
    {
        Rect[] _charBox = new Rect[3];
        Label[] _lblCharNames = new Label[3];
        Label[] _lblCharLvls = new Label[3];
        Button[] _btnPlayChar = new Button[3];
        Button[] _btnDeleteChar = new Button[3];
        Button[] _btnCreateChar = new Button[3];
        PlayerCharacterSprite[] _characterSprites = new PlayerCharacterSprite[3];
        public List<PlayerCharacter> PlayerCharacters;
        private int _elapsedms = 0;
        private GameClient _gameClient;
        public CharacterSelectScene(GameClient gameClient)
        {
            _gameClient = gameClient;
        }

        public override void Initialise(ContentManager content)
        {
            Texture2D baseCharacterTexture = content.Load<Texture2D>("Images/Characters/base");
            Controls = new List<Control>();

            for (int i = 0; i < 3; i++)
            {
                _charBox[i] = new Rect()
                {
                    Name = "rectCharacterBorder" + i,
                    BackColor = Color.White,
                    Size = new Point(200, 200),
                    BorderThickness = 2,
                    BorderColor = Color.Red
                };
                Controls.Add(_charBox[i]);
            }
            _charBox[1].Location = new Point(GameClient.WIDTH / 2 - (_charBox[1].Size.X / 2), GameClient.HEIGHT / 2 - (_charBox[1].Size.Y / 2) - 50);
            _charBox[0].Location = new Point(_charBox[1].Location.X - (_charBox[1].Size.X + (_charBox[1].Size.X / 2)), _charBox[1].Location.Y);
            _charBox[2].Location = new Point(_charBox[1].Location.X + (_charBox[1].Size.X + (_charBox[1].Size.X / 2)), _charBox[1].Location.Y);
            for (int i = 0; i < 3; i++)
            {
                _lblCharNames[i] = new Label()
                {
                    Name = "lblCharName" + i,
                    BackColor = Color.White,
                    FrontColor = Color.Red,
                    Font = GameClient.DEFAULT_FONT,
                    Location = new Point(_charBox[i].Location.X + (_charBox[i].Size.X / 2), _charBox[i].Destination.Top - (int)GameClient.DEFAULT_FONT.MeasureString(" ").Y - 10),
                };
                Controls.Add(_lblCharNames[i]);

                _lblCharLvls[i] = new Label()
                {
                    Name = "lblCharLvl" + i,
                    BackColor = Color.White,
                    FrontColor = Color.Red,
                    Font = GameClient.DEFAULT_FONT,
                    Location = new Point(_charBox[i].Destination.Right, _charBox[i].Destination.Bottom - (int)GameClient.DEFAULT_FONT.MeasureString(" ").Y - 2),
                };
                Controls.Add(_lblCharLvls[i]);

                _btnPlayChar[i] = new Button()
                {
                    IsVisible = false,
                    Name = "btnPlayChar" + i,
                    BackColor = Color.Red,
                    FrontColor = Color.White,
                    Font = GameClient.GW2_FONT,
                    Text = "Play",
                    Size = new Point((int)GameClient.GW2_FONT.MeasureString("Create").X + 10, (int)GameClient.GW2_FONT.MeasureString("Create").Y + 10),
                    Location = new Point(_charBox[i].Destination.Left + (_charBox[i].Size.X / 2) - ((int)GameClient.GW2_FONT.MeasureString("Create").X / 2), _charBox[i].Destination.Bottom + 10)
                };
                _btnPlayChar[i].OnClick += btnPlayChar_OnClick;
                Controls.Add(_btnPlayChar[i]);

                _btnDeleteChar[i] = new Button()
                {
                    IsVisible = false,
                    Name = "btnDeleteChar" + i,
                    BackColor = Color.Red,
                    FrontColor = Color.White,
                    Font = GameClient.GW2_FONT,
                    Text = "Delete",
                    Size = new Point((int)GameClient.GW2_FONT.MeasureString("Create").X + 10, (int)GameClient.GW2_FONT.MeasureString("Create").Y + 10),
                    Location = new Point(_charBox[i].Destination.Left + (_charBox[i].Size.X / 2) - ((int)GameClient.GW2_FONT.MeasureString("Create").X / 2), _btnPlayChar[i].Destination.Bottom + 10)
                };
                _btnDeleteChar[i].OnClick += btnDeleteChar_OnClick;
                Controls.Add(_btnDeleteChar[i]);

                _btnCreateChar[i] = new Button()
                {
                    IsVisible = true,
                    Name = "btnCreateChar" + i,
                    BackColor = Color.Red,
                    FrontColor = Color.White,
                    Font = GameClient.GW2_FONT,
                    Text = "Create",
                    Size = new Point((int)GameClient.GW2_FONT.MeasureString("Create").X + 10, (int)GameClient.GW2_FONT.MeasureString("Create").Y + 10),
                    Location = new Point(_charBox[i].Destination.Left + (_charBox[i].Size.X / 2) - ((int)GameClient.GW2_FONT.MeasureString("Create").X / 2), _charBox[i].Destination.Bottom + 10)
                };
                _btnCreateChar[i].OnClick += btnCreateChar_OnClick;
                Controls.Add(_btnCreateChar[i]);

                _characterSprites[i] = new PlayerCharacterSprite(content, null, null)
                {
                    IsVisible = true,
                    Width = 48,
                    Height = 48,
                    Size = new Point(48, 48),
                    Location = new Point(_charBox[i].Destination.Left + (_charBox[i].Size.X / 2) - 24, _charBox[i].Destination.Top + (_charBox[i].Size.Y / 2) - 24),
                    Texture = baseCharacterTexture
                };
            }
        }

        private void btnDeleteChar_OnClick(object sender, CustomEventArgs.ClickedEventArgs e)
        {
            if (((Control)sender).IsVisible)
            {
                if (sender is Button)
                {
                    Button deleteButton = (Button)sender;
                    int id = Convert.ToInt32(deleteButton.Name[deleteButton.Name.Length - 1]) - 48;
                    PacketBuilder pb = new PacketBuilder(PacketFamily.CHARACTER, PacketAction.DELETE);
                    pb = pb.AddByte((byte)id);
                    GameClient.NetClient.Send(pb.Build());
                }
            }
        }

        /// <summary>
        /// Removes a character from the character select screen character pool
        /// </summary>
        /// <param name="id">id of the character to be removed (0-2 expected)</param>
        internal void RemoveCharacter(byte id)
        {
            if (PlayerCharacters.Count > id)
            {
                PlayerCharacters.RemoveAt(id);
            }

            _characterSprites[id].PlayerCharacter = null;
            _characterSprites[id].IsVisible = false;

            _btnPlayChar[id].IsVisible = false;
            _btnDeleteChar[id].IsVisible = false;
            _btnCreateChar[id].IsVisible = true;

            _lblCharNames[id].Text = "";
            _lblCharLvls[id].Text = "";
        }

        /// <summary>
        /// Create button clicked handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateChar_OnClick(object sender, CustomEventArgs.ClickedEventArgs e)
        {
            //only act if the control is visible
            if (((Control)sender).IsVisible)
            {
                _gameClient.SetState(GameExecution.GameState.CreateCharacter);
            }
        }

        /// <summary>
        /// Play button clicked handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlayChar_OnClick(object sender, CustomEventArgs.ClickedEventArgs e)
        {
            if (((Control)sender).IsVisible)
            {
                int id = Convert.ToInt32(((Control)sender).Name[((Control)sender).Name.Length - 1]) - 48;
                PacketBuilder pb = new PacketBuilder(PacketFamily.PLAY, PacketAction.REQUEST);
                pb = pb.AddByte((byte)id);
                GameClient.NetClient.Send(pb.Build());
            }
        }

        internal void ClearCharacters()
        {
            PlayerCharacters.Clear();
        }

        public void AddCharacter(PlayerCharacter playerCharacter)
        {
            if (PlayerCharacters == null) PlayerCharacters = new List<PlayerCharacter>();
            if (PlayerCharacters.Count >= 3) return;
            PlayerCharacters.Add(playerCharacter);
            _characterSprites[PlayerCharacters.Count - 1].PlayerCharacter = playerCharacter;

            _btnPlayChar[PlayerCharacters.Count - 1].IsVisible = true;
            _btnDeleteChar[PlayerCharacters.Count - 1].IsVisible = true;
            _btnCreateChar[PlayerCharacters.Count - 1].IsVisible = false;

            _lblCharNames[PlayerCharacters.Count - 1].Text = playerCharacter.Name;
            _lblCharLvls[PlayerCharacters.Count - 1].Text = "Lvl " + playerCharacter.Level;

            Vector2 charNameDims = _lblCharNames[PlayerCharacters.Count - 1].Font.MeasureString(playerCharacter.Name);
            Vector2 charLvlDims = _lblCharLvls[PlayerCharacters.Count - 1].Font.MeasureString("Lvl " + playerCharacter.Level);
            _lblCharNames[PlayerCharacters.Count - 1].Size = new Point((int)charNameDims.X, (int)charNameDims.Y);
            _lblCharNames[PlayerCharacters.Count - 1].Location.X = _charBox[PlayerCharacters.Count - 1].Location.X + (_charBox[PlayerCharacters.Count - 1].Size.X / 2) - (int)(charNameDims.X / 2);
            _lblCharLvls[PlayerCharacters.Count - 1].Location.X = _charBox[PlayerCharacters.Count - 1].Destination.Right - (int)(charLvlDims.X);
            _lblCharLvls[PlayerCharacters.Count - 1].Size = new Point((int)charLvlDims.X, (int)charLvlDims.Y);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            foreach (Control control in Controls)
            {
                control.Draw(gameTime, spriteBatch);
            }

            for (int i = 0; i < _characterSprites.Length; i++)
            {
                _characterSprites[i].Draw(gameTime, spriteBatch);
            }
        }

        public override void Update(GameTime gameTime)
        {
            _elapsedms += gameTime.ElapsedGameTime.Milliseconds;
            if (_elapsedms >= 1000)
            {
                if (PlayerCharacters != null)
                {
                    foreach (PlayerCharacter player in PlayerCharacters)
                    {
                        player.Facing = player.Facing + 1;
                        if ((byte)player.Facing == 4) player.Facing = 0;
                    }
                }
                _elapsedms = 0;
            }

            foreach (PlayerCharacterSprite cSprite in _characterSprites)
            {
                cSprite.Update(gameTime, null);
            }

            foreach (Control control in Controls)
            {
                control.Update(gameTime);
            }
        }
    }
}
