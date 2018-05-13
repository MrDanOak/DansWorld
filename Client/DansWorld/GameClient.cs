using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DansWorld.GameClient.GameExecution;
using DansWorld.GameClient.UI;
using DansWorld.GameClient.UI.Game;
using DansWorld.GameClient.UI.CustomEventArgs;
using DansWorld.GameClient.UI.Scenes;
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
        List<Control> _characterSelectControls = new List<Control>();
        public static Net.Client NetClient;
        public static int HEIGHT = 720;
        public static int WIDTH = 1366;
        public static Texture2D DEFAULT_TEXTURE;
        public static SpriteFont DEFAULT_FONT;
        public static SpriteFont DEFAULT_FONT_BOLD;
        public static SpriteFont GW2_FONT;
        public static SpriteFont GW2_FONT_LARGE;
        public static string VERSION = System.Reflection.Assembly.GetExecutingAssembly().
            GetName().Version.ToString();


        public CharacterSelectScene CharacterSelect;
        public MenuScene Menu;
        public RegisterAccountScene RegisterAccount;
        public GameScene GameScence;
        public CharacterCreateScene CharacterCreate;

        string version = System.Reflection.Assembly.GetExecutingAssembly().
            GetName().Version.ToString();

        public int CharacterID = 0;

        public GameClient()
        {
            Window.Title = String.Format("DansWorld - Version {0}", version);
            IsMouseVisible = true; 
            NetClient = new Net.Client("77.101.233.137", 8081, this);
            _graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferHeight = HEIGHT;
            _graphics.PreferredBackBufferWidth = WIDTH;
            Window.AllowAltF4 = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            WIDTH = Window.ClientBounds.Width;
            HEIGHT = Window.ClientBounds.Height;
            _graphics.PreferredBackBufferHeight = HEIGHT;
            _graphics.PreferredBackBufferWidth = WIDTH;
        }

        protected override void Initialize()
        {
            Menu = new MenuScene(this);
            CharacterSelect = new CharacterSelectScene(this);
            RegisterAccount = new RegisterAccountScene(this);
            CharacterCreate = new CharacterCreateScene(this);
            GameScence = new GameScene(this);
            this.Exiting += GameClient_Exiting;
            base.Initialize();
        }

        private void GameClient_Exiting(object sender, EventArgs e)
        {
            NetClient.Stop();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteFont gw2Font = GW2_FONT = Content.Load<SpriteFont>("Fonts/GW2Font");
            SpriteFont defaultFont = DEFAULT_FONT = Content.Load<SpriteFont>("Fonts/arial");
            SpriteFont gw2FontLarge = GW2_FONT_LARGE = Content.Load<SpriteFont>("Fonts/GW2FontLarge");
            SpriteFont defaultBold = DEFAULT_FONT_BOLD =  Content.Load<SpriteFont>("Fonts/arialBold");
            Texture2D baseCharacterTexture = Content.Load<Texture2D>("Images/Characters/base");
            Texture2D enemyTextures = Content.Load<Texture2D>("Images/Characters/enemies");
            DEFAULT_TEXTURE = new Texture2D(GraphicsDevice, 1, 1);
            DEFAULT_TEXTURE.SetData(new Color[] { Color.White });

            Menu.Initialise(Content);
            CharacterSelect.Initialise(Content);
            RegisterAccount.Initialise(Content);
            CharacterCreate.Initialise(Content);
            GameScence.Initialise(Content);
        }

        public void ClearCharacters()
        {
            if (_gameState == GameState.LoggedIn)
                CharacterSelect.ClearCharacters();
            else if (_gameState == GameState.Playing)
                GameScence.ClearCharacters();
        }

        public void AddPlayerCharacters(List<PlayerCharacter> playerCharacters)
        {
            foreach (PlayerCharacter player in playerCharacters) 
                AddPlayerCharacter(player);
        }

        public void AddEnemy(Enemy enemy)
        {
            if (_gameState == GameState.Playing)
            {
                GameScence.AddEnemy(enemy);
            }
        }

        public void AddPlayerCharacter(PlayerCharacter character)
        {
            if (_gameState == GameState.LoggedIn)
                CharacterSelect.AddCharacter(character);
            else if (_gameState == GameState.Playing)
                GameScence.AddCharacter(character);
        }

        public List<PlayerCharacter> GetPlayers()
        {
            if (_gameState == GameState.LoggedIn) return CharacterSelect.PlayerCharacters;
            else if (_gameState == GameState.Playing) return GameScence.PlayerCharacters;
            else return null;
        }

        public List<Enemy> GetEnemies()
        {
            return GameScence.Enemies;
        }

        public void DisplayMessage(string message, string from = "")
        {
            if (_gameState == GameState.MainMenu)
                Menu.DisplayMessage(message);
            else if (_gameState == GameState.RegisterAccount)
                RegisterAccount.DisplayMessage(message);
            else if (_gameState == GameState.CreateCharacter)
                CharacterCreate.DisplayMessage(message);
            else if (_gameState == GameState.Playing)
                GameScence.ShowMessage(message, from);
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
                    break;
            }
        }

        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            switch(_gameState)
            {
                case GameState.MainMenu:
                    Menu.Update(gameTime);
                    break;
                case GameState.LoggedIn:
                    CharacterSelect.Update(gameTime);
                    break;
                case GameState.RegisterAccount:
                    RegisterAccount.Update(gameTime);
                    break;
                case GameState.CreateCharacter:
                    CharacterCreate.Update(gameTime);
                    break;
                case GameState.Playing:
                    GameScence.Update(gameTime);
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
                    Menu.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.LoggedIn:
                    CharacterSelect.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.RegisterAccount:
                    RegisterAccount.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.CreateCharacter:
                    CharacterCreate.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.Playing:
                    GameScence.Draw(gameTime, _spriteBatch);
                    break;
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void SetState(GameState state)
        {
            _gameState = state;
        }

        public void ShowPing(int ms)
        {
            switch (_gameState)
            {
                case GameState.Playing:
                    GameScence.ShowPing(ms);
                    break;
            }
        }

        internal void RemoveCharacter(PlayerCharacter toRemove)
        {
            if (_gameState == GameState.Playing)
                GameScence.RemoveCharacter(toRemove);
        }
    }
}
