using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DansWorld.Common.Net;
using DansWorld.GameClient.GameExecution;
using DansWorld.GameClient.UI.CustomEventArgs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DansWorld.GameClient.UI.Scenes
{
    public class MenuScene : BaseScene
    {
        TextBox _txtUser;
        TextBox _txtPassword;
        Button _btnRegister;
        Button _btnPlay;
        Label _lblDansWorld;
        Label _lblVersion;
        Label _lblMessage;
        GameClient _gameClient;

        public MenuScene(GameClient gameClient)
        {
            _gameClient = gameClient;
        }

        public override void Initialise(ContentManager content)
        {
            Controls = new List<Control>();
            _txtUser = new TextBox()
            {
                Name = "txtUser",
                BackColor = Color.White,
                FrontColor = Color.Black,
                Font = GameClient.DEFAULT_FONT,
                Size = new Point(300, (int)GameClient.DEFAULT_FONT.MeasureString(" ").Y + 10),
                Location = new Point(GameClient.WIDTH / 2 - 150, GameClient.HEIGHT / 2),
                NumbersAllowed = true,
                SpecialCharactersAllowed = false,
                SpacesAllowed = false,
                CharacterLimit = 20
            };
            _txtUser.KeyPressed += TextBox_KeyPressed;
            _txtUser.OnClick += Control_OnClick;
            Controls.Add(_txtUser);

            _lblDansWorld = new Label()
            {
                Name = "lblDansWorld",
                BackColor = Color.White,
                FrontColor = Color.Red,
                Font = GameClient.GW2_FONT_LARGE,
                Text = "Dan's World",
                Size = new Point((int)GameClient.GW2_FONT_LARGE.MeasureString("Dan's World").X + 10, (int)GameClient.GW2_FONT_LARGE.MeasureString("Dan's World").Y + 10),
                Location = new Point(GameClient.WIDTH / 2 - (((int)GameClient.GW2_FONT_LARGE.MeasureString("Dan's World").X + 10) / 2), _txtUser.Destination.Top - ((int)GameClient.GW2_FONT_LARGE.MeasureString("Dan's World").Y + 20))
            };
            Controls.Add(_lblDansWorld);

            _txtPassword = new TextBox()
            {
                Name = "txtPassword",
                BackColor = Color.White,
                FrontColor = Color.Black,
                Font = GameClient.DEFAULT_FONT,
                Size = new Point(300, (int)GameClient.DEFAULT_FONT.MeasureString(" ").Y + 10),
                Location = new Point(GameClient.WIDTH / 2 - 150, _txtUser.Destination.Bottom + 10),
                NumbersAllowed = true,
                SpecialCharactersAllowed = true,
                SpacesAllowed = false,
                IsPasswordField = true
            };
            _txtPassword.OnClick += Control_OnClick;
            _txtPassword.KeyPressed += TextBox_KeyPressed;
            _txtPassword.KeyPressed += TxtPassword_KeyPressed;
            Controls.Add(_txtPassword);

            _btnPlay = new Button()
            {
                Name = "btnPlay",
                BackColor = Color.Red,
                FrontColor = Color.White,
                Font = GameClient.GW2_FONT,
                Text = "Play",
                Size = new Point((int)GameClient.GW2_FONT.MeasureString("Play").X + 10, (int)GameClient.GW2_FONT.MeasureString("Play").Y + 10),
                Location = new Point(_txtPassword.Destination.Right - ((int)GameClient.GW2_FONT.MeasureString("Play").X + 10), _txtPassword.Destination.Bottom + 10)
            };
            _btnPlay.OnClick += PlayButton_OnClick;
            _btnPlay.OnClick += Control_OnClick;
            Controls.Add(_btnPlay);

            _btnRegister = new Button()
            {
                Name = "btnRegister",
                BackColor = Color.Red,
                FrontColor = Color.White,
                Font = GameClient.GW2_FONT,
                Text = "Register",
                Size = new Point((int)GameClient.GW2_FONT.MeasureString("Register").X + 10, (int)GameClient.GW2_FONT.MeasureString("Register").Y + 10),
                Location = new Point(_btnPlay.Destination.Left - ((int)GameClient.GW2_FONT.MeasureString("Register").X + 20), _btnPlay.Destination.Top)
            };
            _btnRegister.OnClick += BtnRegister_OnClick;
            _btnRegister.OnClick += Control_OnClick;
            Controls.Add(_btnRegister);

            _lblMessage = new Label()
            {
                Name = "lblMessage",
                BackColor = Color.White,
                FrontColor = Color.Red,
                Font = GameClient.DEFAULT_FONT_BOLD,
                Text = "",
                Size = new Point((int)GameClient.DEFAULT_FONT_BOLD.MeasureString(String.Format("Version: {0}", GameClient.VERSION)).X + 4, (int)GameClient.DEFAULT_FONT_BOLD.MeasureString(String.Format("Version: {0}", GameClient.VERSION)).Y + 4),
                Location = new Point(GameClient.WIDTH / 2, _btnRegister.Destination.Bottom + 4),
                IsVisible = false
            };
            Controls.Add(_lblMessage);
        }

        private void TextBox_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.KeyPressed == Keys.Tab)
            {
                bool controlFound = false;
                bool focusReset = false;
                while (!focusReset)
                {
                    foreach (Control control in Controls)
                    {
                        if (control is TextBox)
                        {
                            if (controlFound && control.IsVisible)
                            {
                                Focus(control);
                                focusReset = true;
                                break;
                            }
                            if (control == sender)
                            {
                                controlFound = true;
                            }
                        }
                    }
                }
            }
        }
        
        private void TxtPassword_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.KeyPressed == Keys.Enter)
            {
                Login(_txtUser.Text, _txtPassword.Text);
            }
        }

        public void DisplayMessage(string message)
        {
            _lblMessage.IsVisible = true;
            _lblMessage.Text = message;
            _lblMessage.Location = new Point(GameClient.WIDTH / 2 - ((int)_lblMessage.Font.MeasureString(message).X / 2), _lblMessage.Location.Y);
        }

        private void BtnRegister_OnClick(object sender, ClickedEventArgs e)
        {
            _gameClient.SetState(GameState.RegisterAccount);
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

        public override void Update(GameTime gameTime)
        {
            _txtUser.Location = new Point(GameClient.WIDTH / 2 - 150, GameClient.HEIGHT / 2);
            _txtPassword.Location = new Point(GameClient.WIDTH / 2 - 150, _txtUser.Destination.Bottom + 10);
            _btnPlay.Location = new Point(_txtPassword.Destination.Right - ((int)GameClient.GW2_FONT.MeasureString("Play").X + 10), _txtPassword.Destination.Bottom + 10);
            _btnRegister.Location = new Point(_btnPlay.Destination.Left - ((int)GameClient.GW2_FONT.MeasureString("Register").X + 20), _btnPlay.Destination.Top);
            _lblDansWorld.Location = new Point(GameClient.WIDTH / 2 - (((int)GameClient.GW2_FONT_LARGE.MeasureString("Dan's World").X + 10) / 2), _txtUser.Destination.Top - ((int)GameClient.GW2_FONT_LARGE.MeasureString("Dan's World").Y + 20));
            foreach (Control control in Controls)
            {
                control.Update(gameTime);
            }
            base.Update(gameTime);
           
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Control control in Controls)
            {
                control.Draw(gameTime, spriteBatch);
            }
            base.Update(gameTime);
        }

        private void PlayButton_OnClick(object sender, ClickedEventArgs e)
        {
            Console.WriteLine("Play button clicked");
            Login(_txtUser.Text, _txtPassword.Text);
        }

        private void Login(string user, string pass)
        {
            if (!GameClient.NetClient.Connected)
            {
                GameClient.NetClient.Connect();
            }
            PacketBuilder pb = new PacketBuilder(PacketFamily.LOGIN, PacketAction.REQUEST);
            pb = pb.AddByte((byte)_txtUser.Text.Length)
                    .AddString(_txtUser.Text)
                    .AddByte((byte)_txtPassword.Text.Length)
                    .AddString(_txtPassword.Text);
                    
            GameClient.NetClient.Send(pb.Build());
            Console.WriteLine("Attempting to login using user: {0} pass: {1}", _txtUser.Text, _txtPassword.Text);
        }
    }
}
