using System;
using System.Collections.Generic;
using DansWorld.Common.Enums;
using DansWorld.Common.GameEntities;
using DansWorld.Common.IO;
using DansWorld.Common.Net;
using DansWorld.GameClient.UI.CustomEventArgs;
using DansWorld.GameClient.UI.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DansWorld.GameClient.UI.Scenes
{
    public class CharacterCreateScene : BaseScene
    {
        Rect _charBox = new Rect();
        Button _btnCreateChar = new Button();
        Button _btnCancel = new Button();
        Button _btnMale = new Button();
        Button _btnFemale = new Button();
        Button _btnBack = new Button();
        Label _lblError = new Label();
        PlayerCharacterSprite _characterSprite;
        private Label _lblCharacter;
        TextBox _txtCharacterName;
        PlayerCharacter playerCharacter = new PlayerCharacter();
        GameClient _gameClient;
        int _elapsedms = 0;
        public override void Initialise(ContentManager content)
        {
            _characterSprite = new PlayerCharacterSprite(content, null, null);
            Texture2D baseCharacterTexture = content.Load<Texture2D>("Images/Characters/base");
            Controls = new List<Control>();
            
            _charBox = new Rect()
            {
                Name = "rectCharacterBorder",
                BackColor = Color.White,
                Size = new Point(200, 200),
                BorderThickness = 2,
                BorderColor = Color.Red
            };
            Controls.Add(_charBox);
            _charBox.Location = new Point(GameClient.WIDTH / 2 - (_charBox.Size.X / 2), GameClient.HEIGHT / 2 - (_charBox.Size.Y / 2) - 50);

            _btnCreateChar = new Button()
            {
                IsVisible = true,
                Name = "btnCreateChar",
                BackColor = Color.Red,
                FrontColor = Color.White,
                Font = GameClient.GW2_FONT,
                Text = "Create",
                Size = new Point((int)GameClient.GW2_FONT.MeasureString("Create").X + 10, (int)GameClient.GW2_FONT.MeasureString("Create").Y + 10),
                Location = new Point(_charBox.Destination.Left + (_charBox.Size.X / 2) - ((int)GameClient.GW2_FONT.MeasureString("Create").X) - 10, _charBox.Destination.Bottom + 10)
            };
            _btnCreateChar.OnClick += _btnCreateChar_OnClick;
            Controls.Add(_btnCreateChar);

            _btnBack = new Button()
            {
                IsVisible = true,
                Name = "btnBack",
                BackColor = Color.Red,
                FrontColor = Color.White,
                Font = GameClient.GW2_FONT,
                Text = "Back",
                Size = new Point((int)GameClient.GW2_FONT.MeasureString("Back").X + 10, (int)GameClient.GW2_FONT.MeasureString("Back").Y + 10),
                Location = new Point(_charBox.Destination.Left + (_charBox.Size.X / 2) + 10, _charBox.Destination.Bottom + 10)
            };
            _btnBack.OnClick += _btnBack_OnClick;
            Controls.Add(_btnBack);

            _btnMale = new Button()
            {
                IsVisible = true,
                Name = "btnMale",
                BackColor = Color.Red,
                FrontColor = Color.White,
                Font = GameClient.GW2_FONT,
                Text = "Male",
                Size = new Point((int)GameClient.GW2_FONT.MeasureString("Male").X + 10, (int)GameClient.GW2_FONT.MeasureString("Male").Y + 10),
                Location = new Point(_charBox.Destination.Left + (_charBox.Size.X / 2) - ((int)GameClient.GW2_FONT.MeasureString("Male").X ) - 20, _charBox.Destination.Top - 20 - (int)GameClient.GW2_FONT.MeasureString("Male").Y)
            };
            _btnMale.OnClick += _btnMale_OnClick;
            Controls.Add(_btnMale);

            _btnFemale = new Button()
            {
                IsVisible = true,
                Name = "btnFemale",
                BackColor = Color.Red,
                FrontColor = Color.White,
                Font = GameClient.GW2_FONT,
                Text = "Female",
                Size = new Point((int)GameClient.GW2_FONT.MeasureString("Female").X, (int)GameClient.GW2_FONT.MeasureString("Female").Y + 10),
                Location = new Point(_charBox.Destination.Left + (_charBox.Size.X / 2) + 10, _charBox.Destination.Top - 20 - (int)GameClient.GW2_FONT.MeasureString("Female").Y)
            };
            _btnFemale.OnClick += _btnFemale_OnClick;
            Controls.Add(_btnFemale);

            _characterSprite = new PlayerCharacterSprite(content, null, null)
            {
                IsVisible = true,
                Width = 48,
                Height = 48,
                Size = new Point(48, 48),
                Location = new Point(_charBox.Destination.Left + (_charBox.Size.X / 2) - 24, _charBox.Destination.Top + (_charBox.Size.Y / 2) - 24),
                Texture = baseCharacterTexture
            };

            _txtCharacterName = new TextBox()
            {
                Name = "txtCharacterName",
                Text = "",
                BackColor = Color.White,
                FrontColor = Color.Black,
                BorderColor = Color.Black,
                BorderThickness = 2,
                CharacterLimit = 20,
                Font = GameClient.DEFAULT_FONT,
                Size = new Point(300, (int)GameClient.DEFAULT_FONT.MeasureString(" ").Y + 10),
                Location = new Point((GameClient.WIDTH / 2) - 150, _btnFemale.Destination.Top - ((int)GameClient.DEFAULT_FONT.MeasureString(" ").Y + 20)),
                SpacesAllowed = false,
                NumbersAllowed = true,
                SpecialCharactersAllowed = false
            };
            _txtCharacterName.KeyPressed += txt_KeyPressed;
            _txtCharacterName.OnClick += Control_OnClick;
            Controls.Add(_txtCharacterName);

            _lblCharacter = new Label()
            {
                Name = "lblCharacter",
                BackColor = Color.White,
                FrontColor = Color.Red,
                Font = GameClient.DEFAULT_FONT,
                Text = "Name",
                Location = new Point(_txtCharacterName.Location.X, _txtCharacterName.Destination.Top - (int)GameClient.DEFAULT_FONT.MeasureString("Name").Y - 10),
                Size = new Point((int)GameClient.DEFAULT_FONT.MeasureString("Name").X, (int)GameClient.DEFAULT_FONT.MeasureString("Name").Y),
                IsVisible = true
            };
            Controls.Add(_lblCharacter);

            _lblError = new Label()
            {
                Name = "lblError",
                BackColor = Color.White,
                FrontColor = Color.Red,
                Font = GameClient.DEFAULT_FONT_BOLD,
                Text = "",
                Location = new Point(GameClient.WIDTH / 2, _btnBack.Destination.Bottom + (int)GameClient.DEFAULT_FONT.MeasureString("Name").Y),
                Size = new Point((int)GameClient.DEFAULT_FONT.MeasureString("").X, (int)GameClient.DEFAULT_FONT.MeasureString("").Y),
                IsVisible = false
            };
            Controls.Add(_lblError);

            playerCharacter = new PlayerCharacter()
            {
                Gender = Gender.MALE
            };

            _characterSprite.PlayerCharacter = playerCharacter;
        }

        private void _btnBack_OnClick(object sender, ClickedEventArgs e)
        {
            _gameClient.SetState(GameExecution.GameState.LoggedIn);
        }

        private void _btnCreateChar_OnClick(object sender, ClickedEventArgs e)
        {
            PacketBuilder pb = new PacketBuilder(PacketFamily.CHARACTER, PacketAction.CREATE);
            pb = pb.AddByte((byte)_txtCharacterName.Text.Length)
                .AddString(_txtCharacterName.Text)
                .AddByte((byte)playerCharacter.Gender);
            GameClient.NetClient.Send(pb.Build());
            Logger.Log("Character create requested");
        }

        private void _btnFemale_OnClick(object sender, ClickedEventArgs e)
        {
            playerCharacter.Gender = Gender.FEMALE;
        }

        private void _btnMale_OnClick(object sender, ClickedEventArgs e)
        {
            playerCharacter.Gender = Gender.MALE;
        }

        private void txt_KeyPressed(object sender, KeyPressedEventArgs e)
        {

        }

        private void Control_OnClick(object sender, ClickedEventArgs e)
        {
            Focus((Control)sender);
        }

        public void Focus(Control toFocus)
        {
            foreach (Control control in Controls)
            {
                control.HasFocus = (control == toFocus);
                if (control.HasFocus)
                    Console.WriteLine("{0} has focus", control.Name);
            }
        }

        public CharacterCreateScene(GameClient gameClient)
        {
            _gameClient = gameClient;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            foreach (Control control in Controls)
            {
                control.Draw(gameTime, spriteBatch);
            }
            _characterSprite.Draw(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            _elapsedms += gameTime.ElapsedGameTime.Milliseconds;
            if (_elapsedms >= 1000)
            {
                if (playerCharacter != null)
                {
                    playerCharacter.Facing = playerCharacter.Facing + 1;
                    if ((byte)playerCharacter.Facing == 4) playerCharacter.Facing = 0;
                }
                _elapsedms = 0;
            }

            _characterSprite.Update(gameTime, null);

            foreach (Control control in Controls)
            {
                control.Update(gameTime);
            }
        }

        internal void DisplayMessage(string message)
        {
            _lblError.Text = message;
            _lblError.IsVisible = true;
            _lblError.Location = new Point(GameClient.WIDTH / 2 - ((int)GameClient.DEFAULT_FONT.MeasureString(message).X / 2), _lblError.Location.Y);
            _lblError.Size = new Point((int)GameClient.DEFAULT_FONT.MeasureString(message).X, (int)GameClient.DEFAULT_FONT.MeasureString(message).Y);
        }
    }
}
