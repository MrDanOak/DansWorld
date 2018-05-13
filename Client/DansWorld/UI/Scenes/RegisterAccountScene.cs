using System;
using System.Collections.Generic;
using DansWorld.Common.Net;
using DansWorld.GameClient.GameExecution;
using DansWorld.GameClient.UI.CustomEventArgs;
using DansWorld.GameClient.Validation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DansWorld.GameClient.UI.Scenes
{
    public class RegisterAccountScene : BaseScene
    {
        TextBox txtUsername;
        TextBox txtPassword;
        TextBox txtRepeat;
        TextBox txtFullname;
        TextBox txtEmail;
        GameClient _gameClient;
        Label lblUsername;
        Label lblPassword;
        Label lblRepeat;
        Label lblFullname;
        Label lblEmail;
        Label lblDansWorld;
        Label lblMessage;
        Button btnRegister;
        Button btnCancel;
        EmailValidator eValidator = new EmailValidator();

        public RegisterAccountScene(GameClient gameClient)
        {
            _gameClient = gameClient;
        }

        public override void Update(GameTime gameTime)
        {
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
            base.Draw(gameTime, spriteBatch);
        }
        public override void Initialise(ContentManager Content)
        {
            Controls = new List<Control>();
            lblDansWorld = new Label()
            {
                Name = "lblDansWorld",
                BackColor = Color.White,
                FrontColor = Color.Red,
                Font = GameClient.GW2_FONT_LARGE,
                Text = "Dan's World Registration",
                Location = new Point((GameClient.WIDTH / 2) - ((int)GameClient.GW2_FONT_LARGE.MeasureString("Dan's World Registration").X / 4), 50),
                Size = new Point((int)GameClient.DEFAULT_FONT.MeasureString("Dan's World Registration").X, 
                                 (int)GameClient.DEFAULT_FONT.MeasureString("Dan's World Registration").Y),
                IsVisible = true
            };
            Controls.Add(lblDansWorld);

            lblUsername = new Label()
            {
                Name = "lblUsername",
                BackColor = Color.White,
                FrontColor = Color.Red,
                Font = GameClient.DEFAULT_FONT,
                Text = "Username",
                Location = new Point((GameClient.WIDTH / 2), 100),
                Size = new Point((int)GameClient.DEFAULT_FONT.MeasureString("Username").X, (int)GameClient.DEFAULT_FONT.MeasureString("Username").Y),
                IsVisible = true
            };
            Controls.Add(lblUsername);

            txtUsername = new TextBox()
            {
                Name = "txtUsername",
                Text = "",
                BackColor = Color.White,
                FrontColor = Color.Black,
                BorderColor = Color.Black,
                BorderThickness = 2,
                CharacterLimit = 20,
                Font = GameClient.DEFAULT_FONT,
                Size = new Point(300, (int)GameClient.DEFAULT_FONT.MeasureString(" ").Y + 10),
                Location = new Point(lblUsername.Location.X, lblUsername.Destination.Bottom + 10),
                SpacesAllowed = false,
                NumbersAllowed = true,
                SpecialCharactersAllowed = false
            };
            txtUsername.KeyPressed += txt_KeyPressed;
            txtUsername.OnClick += Control_OnClick;
            Controls.Add(txtUsername);

            lblFullname = new Label()
            {
                Name = "lblFullname",
                BackColor = Color.White,
                FrontColor = Color.Red,
                Font = GameClient.DEFAULT_FONT,
                Text = "Fullname",
                Location = new Point(lblUsername.Location.X, txtUsername.Destination.Bottom + 50),
                Size = new Point((int)GameClient.DEFAULT_FONT.MeasureString("Fullname").X, (int)GameClient.DEFAULT_FONT.MeasureString("Fullname").Y),
                IsVisible = true
            };
            Controls.Add(lblFullname);

            txtFullname = new TextBox()
            {
                Name = "txtFullname",
                Text = "",
                BackColor = Color.White,
                FrontColor = Color.Black,
                BorderColor = Color.Black,
                BorderThickness = 2,
                CharacterLimit = 30,
                Font = GameClient.DEFAULT_FONT,
                Size = new Point(300, (int)GameClient.DEFAULT_FONT.MeasureString(" ").Y + 10),
                Location = new Point(lblUsername.Destination.X, lblFullname.Destination.Bottom),
                SpacesAllowed = true,
                NumbersAllowed = true,
                SpecialCharactersAllowed = false
            };
            txtFullname.KeyPressed += txt_KeyPressed;
            txtFullname.OnClick += Control_OnClick;
            Controls.Add(txtFullname);

            lblEmail = new Label()
            {
                Name = "lblEmail",
                BackColor = Color.White,
                FrontColor = Color.Red,
                Font = GameClient.DEFAULT_FONT,
                Text = "Email",
                Location = new Point(lblUsername.Location.X, txtFullname.Destination.Bottom + 50),
                Size = new Point((int)GameClient.DEFAULT_FONT.MeasureString("Email").X, (int)GameClient.DEFAULT_FONT.MeasureString("Email").Y),
                IsVisible = true
            };
            Controls.Add(lblEmail);

            txtEmail = new TextBox()
            {
                Name = "txtEmail",
                Text = "",
                BackColor = Color.White,
                FrontColor = Color.Black,
                BorderColor = Color.Black,
                BorderThickness = 2,
                CharacterLimit = 60,
                Font = GameClient.DEFAULT_FONT,
                Size = new Point(300, (int)GameClient.DEFAULT_FONT.MeasureString(" ").Y + 10),
                Location = new Point(lblUsername.Location.X, lblEmail.Destination.Bottom + 10),
                SpacesAllowed = false,
                NumbersAllowed = true,
                SpecialCharactersAllowed = true 
            };
            txtEmail.KeyPressed += txt_KeyPressed;
            txtEmail.TextChanged += TxtEmail_TextChanged;
            txtEmail.OnClick += Control_OnClick;
            Controls.Add(txtEmail);

            lblPassword = new Label()
            {
                Name = "lblPassword",
                BackColor = Color.White,
                FrontColor = Color.Red,
                Font = GameClient.DEFAULT_FONT,
                Text = "Password",
                Location = new Point(lblUsername.Location.X, txtEmail.Destination.Bottom + 50),
                Size = new Point((int)GameClient.DEFAULT_FONT.MeasureString("Password").X, (int)GameClient.DEFAULT_FONT.MeasureString("Password").Y),
                IsVisible = true
            };
            Controls.Add(lblPassword);

            txtPassword = new TextBox()
            {
                Name = "txtPassword",
                Text = "",
                BackColor = Color.White,
                FrontColor = Color.Black,
                BorderColor = Color.Black,
                BorderThickness = 2,
                CharacterLimit = 20,
                Font = GameClient.DEFAULT_FONT,
                Size = new Point(300, (int)GameClient.DEFAULT_FONT.MeasureString(" ").Y + 10),
                Location = new Point(lblUsername.Location.X, lblPassword.Destination.Bottom + 10),
                SpacesAllowed = false,
                NumbersAllowed = true,
                SpecialCharactersAllowed = true,
                IsPasswordField = true
            };
            txtPassword.KeyPressed += txt_KeyPressed;
            txtPassword.OnClick += Control_OnClick;
            Controls.Add(txtPassword);

            lblRepeat = new Label()
            {
                Name = "lblRepeat",
                BackColor = Color.White,
                FrontColor = Color.Red,
                Font = GameClient.DEFAULT_FONT,
                Text = "Repeat",
                Location = new Point(lblUsername.Location.X, txtPassword.Destination.Bottom + 50),
                Size = new Point((int)GameClient.DEFAULT_FONT.MeasureString("Repeat").X, (int)GameClient.DEFAULT_FONT.MeasureString("Repeat").Y),
                IsVisible = true
            };
            Controls.Add(lblRepeat);

            txtRepeat = new TextBox()
            {
                Name = "txtRepeat",
                Text = "",
                BackColor = Color.White,
                FrontColor = Color.Black,
                BorderColor = Color.Black,
                BorderThickness = 2,
                CharacterLimit = 20,
                Font = GameClient.DEFAULT_FONT,
                Size = new Point(300, (int)GameClient.DEFAULT_FONT.MeasureString(" ").Y + 10),
                Location = new Point(lblUsername.Location.X, lblRepeat.Destination.Bottom + 10),
                SpacesAllowed = false,
                NumbersAllowed = true,
                SpecialCharactersAllowed = true,
                IsPasswordField = true
            };
            txtRepeat.KeyPressed += txt_KeyPressed;
            txtRepeat.OnClick += Control_OnClick;
            Controls.Add(txtRepeat);

            btnRegister = new Button()
            {
                Text = "Register",
                BackColor = Color.Red,
                FrontColor = Color.White,
                Font = GameClient.GW2_FONT,
                IsVisible = true,
                Name = "btnRegister",
                Location = new Point(txtRepeat.Destination.Right - (int)GameClient.GW2_FONT.MeasureString("Register").X, txtRepeat.Destination.Bottom + 20),
                Size = new Point((int)GameClient.GW2_FONT.MeasureString("Register").X+10, (int)GameClient.GW2_FONT.MeasureString("Register").Y+10)
            };
            btnRegister.OnClick += BtnRegister_OnClick;
            btnRegister.OnClick += Control_OnClick;
            Controls.Add(btnRegister);

            btnCancel = new Button()
            {
                Text = "Cancel",
                BackColor = Color.Red,
                FrontColor = Color.White,
                Font = GameClient.GW2_FONT,
                IsVisible = true,
                Name = "btnCancel",
                Location = new Point(btnRegister.Destination.X - (int)GameClient.GW2_FONT.MeasureString("Cancel").X - 20, txtRepeat.Destination.Bottom + 20),
                Size = new Point((int)GameClient.GW2_FONT.MeasureString("Cancel").X + 10, (int)GameClient.GW2_FONT.MeasureString("Cancel").Y + 10)
            };
            btnCancel.OnClick += Control_OnClick;
            btnCancel.OnClick += BtnCancel_OnClick;
            Controls.Add(btnCancel);

            lblMessage = new Label()
            {
                Name = "lblMessage",
                BackColor = Color.White,
                FrontColor = Color.Red,
                Font = GameClient.DEFAULT_FONT_BOLD,
                Text = "Message",
                Location = new Point(lblUsername.Location.X, btnCancel.Destination.Bottom + 10),
                Size = new Point((int)GameClient.DEFAULT_FONT_BOLD.MeasureString("Message").X, (int)GameClient.DEFAULT_FONT_BOLD.MeasureString("Message").Y),
                IsVisible = false
            };
            Controls.Add(lblMessage);

        }

        private void txt_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.KeyPressed == Microsoft.Xna.Framework.Input.Keys.Tab)
            {
                bool controlFound = false;
                bool focusChanged = false;
                bool iteratedOnce = false;
                while (!focusChanged)
                {
                    foreach (Control control in Controls)
                    {
                        if (control is TextBox)
                        {
                            // if a control was found chronologically we set the focus to the next textbox in the collection
                            // if not and we've iterated once, we'll just set the focus to the first text box.
                            if (controlFound || iteratedOnce)
                            {
                                control.HasFocus = true;
                                focusChanged = true;
                                break;
                            }

                            if (control == sender)
                            {
                                controlFound = true;
                            }
                        }
                    }
                    iteratedOnce = true;
                }
                if (controlFound) ((TextBox)sender).HasFocus = false;
            }
        }

        internal void DisplayMessage(string message)
        {
            lblMessage.IsVisible = true;
            lblMessage.Text = message;
        }

        private void BtnCancel_OnClick(object sender, ClickedEventArgs e)
        {
            _gameClient.SetState(GameState.MainMenu);
        }

        private void BtnRegister_OnClick(object sender, ClickedEventArgs e)
        {
            if (txtPassword.Text == txtRepeat.Text && eValidator.Validate(txtEmail.Text)) 
            {
                if (!GameClient.NetClient.Connected)
                {
                    GameClient.NetClient.Connect();
                }

                PacketBuilder pb = new PacketBuilder(PacketFamily.REGISTER, PacketAction.REQUEST);
                pb = pb.AddByte((byte)txtUsername.Text.Length)
                       .AddString(txtUsername.Text)
                       .AddByte((byte)txtPassword.Text.Length)
                       .AddString(txtPassword.Text)
                       .AddByte((byte)txtEmail.Text.Length)
                       .AddString(txtEmail.Text)
                       .AddByte((byte)txtFullname.Text.Length)
                       .AddString(txtFullname.Text);
                GameClient.NetClient.Send(pb.Build());
            }
        }

        private void TxtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!eValidator.Validate(e.Text))
            {
                txtEmail.BorderColor = Color.Red;
            }
            else
            {
                txtEmail.BorderColor = Color.Black;
            }
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
    }
}
