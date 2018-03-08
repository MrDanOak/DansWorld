using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DansWorld.Common.Enums;
using DansWorld.Common.GameEntities;
using DansWorld.GameClient.UI.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DansWorld.GameClient.UI.Scenes
{
    class CharacterSelectScene : BaseScene
    {
        Rect[] _charBox = new Rect[3];
        Label[] _lblCharNames = new Label[3];
        Label[] _lblCharLvls = new Label[3];
        Button[] _btnPlayChar = new Button[3];
        Button[] _btnDeleteChar = new Button[3];
        Button[] _btnCreateChar = new Button[3];
        CharacterSprite[] _characterSprite = new CharacterSprite[3];
        public List<Character> Characters;
        private int _elapsedms = 0;

        public override void Initialise(ContentManager Content)
        {
            Texture2D baseCharacterTexture = Content.Load<Texture2D>("Images/Characters/base");
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
                Controls.Add(_btnCreateChar[i]);

                _characterSprite[i] = new CharacterSprite()
                {
                    IsVisible = true,
                    SpriteWidth = 48,
                    SpriteHeight = 48,
                    Size = new Point(48, 48),
                    Location = new Point(_charBox[i].Destination.Left + (_charBox[i].Size.X / 2) - 24, _charBox[i].Destination.Top + (_charBox[i].Size.Y / 2) - 24),
                    Name = "charSprite" + i,
                    FrontColor = Color.White,
                    BaseTexture = baseCharacterTexture
                };
                Controls.Add(_characterSprite[i]);
            }
        }

        internal void ClearCharacters()
        {
            Characters.Clear();
        }

        public void AddCharacter(Character character)
        {
            if (Characters == null) Characters = new List<Character>();
            if (Characters.Count >= 3) return;
            Characters.Add(character);
            _characterSprite[Characters.Count - 1].Character = character;

            _btnPlayChar[Characters.Count - 1].IsVisible = true;
            _btnDeleteChar[Characters.Count - 1].IsVisible = true;
            _btnCreateChar[Characters.Count - 1].IsVisible = false;

            _lblCharNames[Characters.Count - 1].Text = character.Name;
            _lblCharLvls[Characters.Count - 1].Text = "Lvl " + character.Level;

            Vector2 charNameDims = _lblCharNames[Characters.Count - 1].Font.MeasureString(character.Name);
            Vector2 charLvlDims = _lblCharLvls[Characters.Count - 1].Font.MeasureString("Lvl " + character.Level);
            _lblCharNames[Characters.Count - 1].Size = new Point((int)charNameDims.X, (int)charNameDims.Y);
            _lblCharNames[Characters.Count - 1].Location.X -= (int)(charNameDims.X / 2);
            _lblCharLvls[Characters.Count - 1].Location.X -= (int)(charLvlDims.X);
            _lblCharLvls[Characters.Count - 1].Size = new Point((int)charLvlDims.X, (int)charLvlDims.Y);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Control control in Controls)
            {
                control.Draw(gameTime, spriteBatch);
            }
            base.Draw(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            _elapsedms += gameTime.ElapsedGameTime.Milliseconds;
            if (_elapsedms >= 1000)
            {
                foreach (Character character in Characters)
                {
                    character.SetFacing((Direction)character.Facing + 1);
                }
                _elapsedms = 0;
            }
            foreach (Control control in Controls)
            {
                control.Update(gameTime);
            }
            base.Update(gameTime);
        }
    }
}
