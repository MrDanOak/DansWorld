using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DansWorld.GameClient.GameExecution;
using DansWorld.GameClient.UI;
using DansWorld.GameClient.UI.Game;
using DansWorld.GameClient.UI.CustomEventArgs;
using DansWorld.Common.Net;
using DansWorld.Common.GameEntities;
using System.Collections.Generic;
using System;

namespace DansWorld.GameClient
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameClient : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        GameState _gameState = GameState.MainMenu;
        List<Control> _mainMenuControls = new List<Control>();
        List<Control> _characterSelectControls = new List<Control>();
        public static Net.Client NetClient;
        public static List<Character> Characters;
        public const int HEIGHT = 720;
        public const int WIDTH = 1366;
        public static Texture2D DEFAULT_TEXTURE;
        public static SpriteFont DEFAULT_FONT;
        public static SpriteFont DEFAULT_FONT_BOLD;
        public static SpriteFont GW2_FONT;
        public static SpriteFont GW2_FONT_LARGE;
        public static string VERSION = System.Reflection.Assembly.GetExecutingAssembly().
            GetName().Version.ToString();
        TextBox txtUser;
        TextBox txtPassword;
        Button btnCreate;
        Button btnPlay;
        Label lblDansWorld;
        Label lblVersion;
        Label lblMessage;
        Rect[] charBox = new Rect[3];
        Label[] lblCharNames = new Label[3];
        Label[] lblCharLvls = new Label[3];
        Button[] btnPlayChar = new Button[3];
        Button[] btnDeleteChar = new Button[3];
        Button[] btnCreateChar = new Button[3];
        CharacterSprite[] characterSprite = new CharacterSprite[3];

        MenuScene menu = new MenuScene();

        string version = System.Reflection.Assembly.GetExecutingAssembly().
            GetName().Version.ToString();

        public GameClient()
        {
            Window.Title = String.Format("DansWorld - Version {0}", version);
            IsMouseVisible = true; 
            NetClient = new Net.Client("127.0.0.1", 8081, this);
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
            SpriteFont gw2Font = GW2_FONT = Content.Load<SpriteFont>("Fonts/GW2Font");
            SpriteFont defaultFont = DEFAULT_FONT = Content.Load<SpriteFont>("Fonts/arial");
            SpriteFont gw2FontLarge = GW2_FONT_LARGE = Content.Load<SpriteFont>("Fonts/GW2FontLarge");
            SpriteFont defaultBold = DEFAULT_FONT_BOLD =  Content.Load<SpriteFont>("Fonts/arialBold");
            Texture2D baseCharacterTexture = Content.Load<Texture2D>("Images/Characters/base");
            DEFAULT_TEXTURE = new Texture2D(GraphicsDevice, 1, 1);
            DEFAULT_TEXTURE.SetData(new Color[] { Color.White });

            menu.Initialise();

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

            lblMessage = new Label()
            {
                Name = "lblMessage",
                BackColor = Color.White,
                FrontColor = Color.Red,
                Font = defaultBold,
                Text = "",
                Size = new Point((int)defaultBold.MeasureString(String.Format("Version: {0}", version)).X + 4, (int)defaultBold.MeasureString(String.Format("Version: {0}", version)).Y + 4),
                Location = new Point(WIDTH / 2, btnCreate.Destination.Bottom + 4),
                IsVisible = false
            };
            _mainMenuControls.Add(lblMessage);

            for (int i = 0; i < 3; i++)
            {
                charBox[i] = new Rect()
                {
                    Name = "rectCharacterBorder" + i,
                    BackColor = Color.White,
                    Size = new Point(200, 200),
                    BorderThickness = 2, 
                    BorderColor = Color.Red
                };
                _characterSelectControls.Add(charBox[i]);
            }
            charBox[1].Location = new Point(WIDTH / 2 - (charBox[1].Size.X / 2), HEIGHT / 2 - (charBox[1].Size.Y / 2) - 50);
            charBox[0].Location = new Point(charBox[1].Location.X - (charBox[1].Size.X + (charBox[1].Size.X /2)), charBox[1].Location.Y);
            charBox[2].Location = new Point(charBox[1].Location.X + (charBox[1].Size.X + (charBox[1].Size.X / 2)), charBox[1].Location.Y);
            for (int i = 0; i < 3; i++)
            {
                lblCharNames[i] = new Label()
                {
                    Name = "lblCharName" + i,
                    BackColor = Color.White,
                    FrontColor = Color.Red,
                    Font = defaultFont,
                    Location = new Point(charBox[i].Location.X + (charBox[i].Size.X / 2), charBox[i].Destination.Top - (int)defaultFont.MeasureString(" ").Y - 10), 
                };
                _characterSelectControls.Add(lblCharNames[i]);

                lblCharLvls[i] = new Label()
                {
                    Name = "lblCharLvl" + i,
                    BackColor = Color.White,
                    FrontColor = Color.Red,
                    Font = defaultFont,
                    Location = new Point(charBox[i].Destination.Right, charBox[i].Destination.Bottom - (int)defaultFont.MeasureString(" ").Y - 2),
                };
                _characterSelectControls.Add(lblCharLvls[i]);

                btnPlayChar[i] = new Button()
                {
                    IsVisible = false,
                    Name = "btnPlayChar" + i,
                    BackColor = Color.Red,
                    FrontColor = Color.White,
                    Font = gw2Font,
                    Text = "Play",
                    Size = new Point((int)gw2Font.MeasureString("Create").X + 10, (int)gw2Font.MeasureString("Create").Y + 10),
                    Location = new Point(charBox[i].Destination.Left + (charBox[i].Size.X / 2) - ((int)gw2Font.MeasureString("Create").X / 2), charBox[i].Destination.Bottom + 10)
                };
                _characterSelectControls.Add(btnPlayChar[i]);

                btnDeleteChar[i] = new Button()
                {
                    IsVisible = false,
                    Name = "btnDeleteChar" + i,
                    BackColor = Color.Red,
                    FrontColor = Color.White,
                    Font = gw2Font,
                    Text = "Delete",
                    Size = new Point((int)gw2Font.MeasureString("Create").X + 10, (int)gw2Font.MeasureString("Create").Y + 10),
                    Location = new Point(charBox[i].Destination.Left + (charBox[i].Size.X / 2) - ((int)gw2Font.MeasureString("Create").X / 2), btnPlayChar[i].Destination.Bottom + 10)
                };
                _characterSelectControls.Add(btnDeleteChar[i]);

                btnCreateChar[i] = new Button()
                {
                    IsVisible = true,
                    Name = "btnCreateChar" + i,
                    BackColor = Color.Red,
                    FrontColor = Color.White,
                    Font = gw2Font,
                    Text = "Create",
                    Size = new Point((int)gw2Font.MeasureString("Create").X + 10, (int)gw2Font.MeasureString("Create").Y + 10),
                    Location = new Point(charBox[i].Destination.Left + (charBox[i].Size.X / 2) - ((int)gw2Font.MeasureString("Create").X / 2), charBox[i].Destination.Bottom + 10)
                };
                _characterSelectControls.Add(btnCreateChar[i]);

                characterSprite[i] = new CharacterSprite()
                {
                    IsVisible = true,
                    SpriteWidth = 48, 
                    SpriteHeight = 48, 
                    Size = new Point(48,48), 
                    Location = new Point(charBox[i].Destination.Left + (charBox[i].Size.X / 2) - 24, charBox[i].Destination.Top + (charBox[i].Size.Y / 2) - 24),
                    Name = "charSprite" + i,
                    FrontColor = Color.White, 
                    BaseTexture = baseCharacterTexture
                };
                _characterSelectControls.Add(characterSprite[i]);
            }

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

        public void ClearCharacters()
        {
            Characters.Clear();
        }

        public void AddCharacters(List<Character> characters)
        {
            foreach (Character character in characters) 
                AddCharacter(character);
        }

        public void AddCharacter(Character character)
        {
            if (Characters == null) Characters = new List<Character>();
            if (Characters.Count >= 3) return;
            Characters.Add(character);
            characterSprite[Characters.Count - 1].Character = character;

            btnPlayChar[Characters.Count - 1].IsVisible = true;
            btnDeleteChar[Characters.Count - 1].IsVisible = true;
            btnCreateChar[Characters.Count - 1].IsVisible = false;

            lblCharNames[Characters.Count - 1].Text = character.Name;
            lblCharLvls[Characters.Count - 1].Text = "Lvl " + character.Level;

            Vector2 charNameDims = lblCharNames[Characters.Count - 1].Font.MeasureString(character.Name);
            Vector2 charLvlDims = lblCharLvls[Characters.Count - 1].Font.MeasureString("Lvl " + character.Level);
            lblCharNames[Characters.Count - 1].Size = new Point((int)charNameDims.X, (int)charNameDims.Y);
            lblCharNames[Characters.Count - 1].Location.X -= (int)(charNameDims.X / 2);
            lblCharLvls[Characters.Count - 1].Location.X -= (int)(charLvlDims.X);
            lblCharLvls[Characters.Count - 1].Size = new Point((int)charLvlDims.X, (int)charLvlDims.Y);
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
                                    if (controlFound && control.IsVisible)
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

        public void DisplayLoginMessage(string message)
        {
            lblMessage.IsVisible = true;
            lblMessage.Text = message;
            lblMessage.Location = new Point(WIDTH / 2 - ((int)lblMessage.Font.MeasureString(message).X / 2), lblMessage.Location.Y);
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
            if (!NetClient.Connected)
            {
                NetClient.Connect();
            }
            PacketBuilder pb = new PacketBuilder(PacketFamily.Login, PacketAction.Request);
            pb = pb.AddString("u:"+txtUser.Text)
                   .AddString("p:"+txtPassword.Text);
            NetClient.Send(pb.Build());
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
                    menu.Update(gameTime);
                    /*foreach (Control control in _mainMenuControls)
                    {
                        control.Update(gameTime);
                    }*/
                    break;
                case GameState.LoggedIn:
                    foreach (Control control in _characterSelectControls)
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
                    menu.Draw(gameTime, _spriteBatch);
                    /*foreach (Control control in _mainMenuControls)
                    {
                        control.Draw(gameTime, _spriteBatch);
                    }*/
                    break;
                case GameState.LoggedIn:
                    foreach (Control control in _characterSelectControls)
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

        public void SetState(GameState state)
        {
            _gameState = state;
        }
    }
}
