using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DansWorld.GameExecution;
using DansWorld.UI;
using DansWorld.UI.CustomEventArgs;
using System.Collections.Generic;
using System;

namespace DansWorld
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class DansWorld : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        GameState _gameState = GameState.MainMenu;
        List<Control> _mainMenuControls = new List<Control>();

        public const int HEIGHT = 720;
        public const int WIDTH = 1366;
        public static Texture2D DEFAULT_TEXTURE;
        TextBox txtUser;
        TextBox txtPassword;
        Button btnCreate;
        Button btnPlay;
        Label lblDansWorld;
        Label lblVersion;
        string version = System.Reflection.Assembly.GetExecutingAssembly().
            GetName().Version.ToString();

        public DansWorld()
        {
            Window.Title = String.Format("DansWorld - Version {0}", version);
            IsMouseVisible = true; 

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferHeight = HEIGHT;
            _graphics.PreferredBackBufferWidth = WIDTH;

        }
        protected override void Initialize()
        {

            base.Initialize();
        }
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteFont gw2Font = Content.Load<SpriteFont>("Fonts/GW2Font");
            SpriteFont defaultFont = Content.Load<SpriteFont>("Fonts/arial");
            SpriteFont gw2FontLarge = Content.Load<SpriteFont>("Fonts/GW2FontLarge");
            SpriteFont defaultBold = Content.Load<SpriteFont>("Fonts/arialBold");

            DEFAULT_TEXTURE = new Texture2D(GraphicsDevice, 1, 1);
            DEFAULT_TEXTURE.SetData(new Color[] { Color.White });
            txtUser = new TextBox()
            {
                Name = "txtUser",
                BackColor = Color.White,
                FrontColor = Color.Black,
                Font = defaultFont,
                Size = new Point(300, (int)defaultFont.MeasureString(" ").Y + 10),
                Location = new Point(WIDTH / 2 - 150, HEIGHT / 2),
                NumbersAllowed = true,
                SpecialCharactersAllowed = false,
                SpacesAllowed = false,
                CharacterLimit = 5
            };
            txtUser.KeyPressed += TextBox_KeyPressed;
            txtUser.OnClick += Control_OnClick;
            _mainMenuControls.Add(txtUser);

            lblDansWorld = new Label()
            {
                Name = "lblDansWorld",
                BackColor = Color.White,
                FrontColor = Color.Red,
                Font = gw2FontLarge,
                Text = "Dan's World",
                Size = new Point((int)gw2FontLarge.MeasureString("Dan's World").X + 10, (int)gw2FontLarge.MeasureString("Dan's World").Y + 10),
                Location = new Point(WIDTH / 2 - (((int)gw2FontLarge.MeasureString("Dan's World").X + 10) / 2), txtUser.Destination.Top - ((int)gw2FontLarge.MeasureString("Dan's World").Y + 20))
            };
            _mainMenuControls.Add(lblDansWorld);

            txtPassword = new TextBox()
            {
                Name = "txtPassword",
                BackColor = Color.White,
                FrontColor = Color.Black,
                Font = defaultFont,
                Size = new Point(300, (int)defaultFont.MeasureString(" ").Y + 10),
                Location = new Point(WIDTH / 2 - 150, txtUser.Destination.Bottom + 10),
                NumbersAllowed = true,
                SpecialCharactersAllowed = false,
                SpacesAllowed = false,
                IsPasswordField = true
            };
            txtPassword.OnClick += Control_OnClick;
            txtPassword.KeyPressed += TextBox_KeyPressed;
            txtPassword.KeyPressed += TxtPassword_KeyPressed;
            _mainMenuControls.Add(txtPassword);

            btnPlay = new Button()
            {
                Name = "btnPlay",
                BackColor = Color.Red,
                FrontColor = Color.White,
                Font = gw2Font,
                Text = "Play",
                Size = new Point((int)gw2Font.MeasureString("Play").X + 10, (int)gw2Font.MeasureString("Play").Y + 10),
                Location = new Point(txtPassword.Destination.Right - ((int)gw2Font.MeasureString("Play").X + 10), txtPassword.Destination.Bottom + 10)
            };
            btnPlay.OnClick += PlayButton_OnClick;
            btnPlay.OnClick += Control_OnClick;
            _mainMenuControls.Add(btnPlay);

            btnCreate = new Button()
            {
                Name = "btnCreate",
                BackColor = Color.Red,
                FrontColor = Color.White,
                Font = gw2Font,
                Text = "Create",
                Size = new Point((int)gw2Font.MeasureString("Create").X + 10, (int)gw2Font.MeasureString("Create").Y + 10),
                Location = new Point(btnPlay.Destination.Left - ((int)gw2Font.MeasureString("Create").X + 20), btnPlay.Destination.Top)
            };
            btnCreate.OnClick += BtnCreate_OnClick;
            btnCreate.OnClick += Control_OnClick;
            _mainMenuControls.Add(btnCreate);

            lblVersion = new Label()
            {
                Name = "lblVersion",
                BackColor = Color.White,
                FrontColor = Color.Red,
                Font = defaultBold,
                Text = String.Format("Version: {0}", version),
                Size = new Point((int)defaultBold.MeasureString(String.Format("Version: {0}", version)).X + 4, (int)defaultBold.MeasureString(String.Format("Version: {0}", version)).Y + 4),
                Location = new Point(WIDTH - ((int)defaultBold.MeasureString(String.Format("Version: {0}", version)).X + 4), HEIGHT - ((int)defaultBold.MeasureString(String.Format("Version: {0}", version)).Y + 4))
            };
            _mainMenuControls.Add(lblVersion);
            Focus(txtUser, _mainMenuControls);
        }

        private void TextBox_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.KeyPressed == Keys.Tab)
            {
                switch (_gameState)
                {
                    case GameState.MainMenu:
                        bool controlFound = false;
                        bool focusReset = false;
                        while (!focusReset)
                        {
                            foreach (Control control in _mainMenuControls)
                            {
                                if (control is TextBox)
                                {
                                    if (controlFound)
                                    {
                                        Focus(control, _mainMenuControls);
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
                        break;
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

        private void Login(string user, string pass)
        {
            Console.WriteLine("Attempting to login using user: {0} pass: {1}", txtUser.Text, txtPassword.Text);
        }

        private void BtnCreate_OnClick(object sender, ClickedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Focus(Control toFocus, List<Control> controlSet)
        {
            foreach (Control control in controlSet)
            {
                control.HasFocus = (control == toFocus);
                if (control.HasFocus)
                    Console.WriteLine("{0} has focus", control.Name);
            }
        }

        private void Control_OnClick(object sender, ClickedEventArgs e)
        {
            switch (_gameState)
            {
                case GameState.MainMenu:
                    Focus((Control)sender, _mainMenuControls);
                    break;
            }
        }

        private void PlayButton_OnClick(object sender, ClickedEventArgs e)
        {
            Console.WriteLine("Play button clicked");
            Login(txtUser.Text, txtPassword.Text);
        }

        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            switch(_gameState)
            {
                case GameState.MainMenu:
                    foreach(Control control in _mainMenuControls)
                    {
                        control.Update(gameTime);
                    }
                    break;
                case GameState.Playing:
                    break;
            }

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin();
            switch (_gameState)
            {
                case GameState.MainMenu:
                    foreach (Control control in _mainMenuControls)
                    {
                        control.Draw(gameTime, _spriteBatch);
                    }
                    break;
                case GameState.Playing:
                    break;
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
