using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DansWorld.Common.Net;
using DansWorld.GameClient.UI.CustomEventArgs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DansWorld.GameClient.UI
{
    public class MenuScene : Scene
    {
        TextBox txtUser;
        TextBox txtPassword;
        Button btnCreate;
        Button btnPlay;
        Label lblDansWorld;
        Label lblVersion;
        Label lblMessage;
        public MenuScene()
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Control control in Controls)
            {
                control.Draw(gameTime, spriteBatch);
            }
        }

        public override void Initialise()
        {
            Controls = new List<Control>();
            txtUser = new TextBox()
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
                CharacterLimit = 5
            };
            txtUser.KeyPressed += TextBox_KeyPressed;
            txtUser.OnClick += Control_OnClick;
            Controls.Add(txtUser);

            lblDansWorld = new Label()
            {
                Name = "lblDansWorld",
                BackColor = Color.White,
                FrontColor = Color.Red,
                Font = GameClient.GW2_FONT_LARGE,
                Text = "Dan's World",
                Size = new Point((int)GameClient.GW2_FONT_LARGE.MeasureString("Dan's World").X + 10, (int)GameClient.GW2_FONT_LARGE.MeasureString("Dan's World").Y + 10),
                Location = new Point(GameClient.WIDTH / 2 - (((int)GameClient.GW2_FONT_LARGE.MeasureString("Dan's World").X + 10) / 2), txtUser.Destination.Top - ((int)GameClient.GW2_FONT_LARGE.MeasureString("Dan's World").Y + 20))
            };
            Controls.Add(lblDansWorld);

            txtPassword = new TextBox()
            {
                Name = "txtPassword",
                BackColor = Color.White,
                FrontColor = Color.Black,
                Font = GameClient.DEFAULT_FONT,
                Size = new Point(300, (int)GameClient.DEFAULT_FONT.MeasureString(" ").Y + 10),
                Location = new Point(GameClient.WIDTH / 2 - 150, txtUser.Destination.Bottom + 10),
                NumbersAllowed = true,
                SpecialCharactersAllowed = false,
                SpacesAllowed = false,
                IsPasswordField = true
            };
            txtPassword.OnClick += Control_OnClick;
            txtPassword.KeyPressed += TextBox_KeyPressed;
            txtPassword.KeyPressed += TxtPassword_KeyPressed;
            Controls.Add(txtPassword);

            btnPlay = new Button()
            {
                Name = "btnPlay",
                BackColor = Color.Red,
                FrontColor = Color.White,
                Font = GameClient.GW2_FONT,
                Text = "Play",
                Size = new Point((int)GameClient.GW2_FONT.MeasureString("Play").X + 10, (int)GameClient.GW2_FONT.MeasureString("Play").Y + 10),
                Location = new Point(txtPassword.Destination.Right - ((int)GameClient.GW2_FONT.MeasureString("Play").X + 10), txtPassword.Destination.Bottom + 10)
            };
            btnPlay.OnClick += PlayButton_OnClick;
            btnPlay.OnClick += Control_OnClick;
            Controls.Add(btnPlay);

            btnCreate = new Button()
            {
                Name = "btnCreate",
                BackColor = Color.Red,
                FrontColor = Color.White,
                Font = GameClient.GW2_FONT,
                Text = "Create",
                Size = new Point((int)GameClient.GW2_FONT.MeasureString("Create").X + 10, (int)GameClient.GW2_FONT.MeasureString("Create").Y + 10),
                Location = new Point(btnPlay.Destination.Left - ((int)GameClient.GW2_FONT.MeasureString("Create").X + 20), btnPlay.Destination.Top)
            };
            btnCreate.OnClick += BtnCreate_OnClick;
            btnCreate.OnClick += Control_OnClick;
            Controls.Add(btnCreate);

            lblMessage = new Label()
            {
                Name = "lblMessage",
                BackColor = Color.White,
                FrontColor = Color.Red,
                Font = GameClient.DEFAULT_FONT_BOLD,
                Text = "",
                Size = new Point((int)GameClient.DEFAULT_FONT_BOLD.MeasureString(String.Format("Version: {0}", GameClient.VERSION)).X + 4, (int)GameClient.DEFAULT_FONT_BOLD.MeasureString(String.Format("Version: {0}", GameClient.VERSION)).Y + 4),
                Location = new Point(GameClient.WIDTH / 2, btnCreate.Destination.Bottom + 4),
                IsVisible = false
            };
            Controls.Add(lblMessage);
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
                Login(txtUser.Text, txtPassword.Text);
            }
        }

        private void BtnCreate_OnClick(object sender, ClickedEventArgs e)
        {
            throw new NotImplementedException();
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
            foreach (Control control in Controls)
            {
                control.Update(gameTime);
            }
        }

        private void PlayButton_OnClick(object sender, ClickedEventArgs e)
        {
            Console.WriteLine("Play button clicked");
            Login(txtUser.Text, txtPassword.Text);
        }

        private void Login(string user, string pass)
        {
            if (!GameClient.NetClient.Connected)
            {
                GameClient.NetClient.Connect();
            }
            PacketBuilder pb = new PacketBuilder(PacketFamily.Login, PacketAction.Request);
            pb = pb.AddString("u:" + txtUser.Text)
                   .AddString("p:" + txtPassword.Text);
            GameClient.NetClient.Send(pb.Build());
            Console.WriteLine("Attempting to login using user: {0} pass: {1}", txtUser.Text, txtPassword.Text);
        }
    }
}
