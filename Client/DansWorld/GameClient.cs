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


        CharacterSelectScene characterSelect;
        MenuScene menu;
        RegisterAccountScene registerAccount;
        GameScene gameScene;

        string version = System.Reflection.Assembly.GetExecutingAssembly().
            GetName().Version.ToString();

        public int CharacterID = 0;

        public GameClient()
        {
            Window.Title = String.Format("DansWorld - Version {0}", version);
            IsMouseVisible = true; 
            NetClient = new Net.Client("127.0.0.1", 8081, this);
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
            menu = new MenuScene(this);
            characterSelect = new CharacterSelectScene(this);
            registerAccount = new RegisterAccountScene(this);
            gameScene = new GameScene(this);
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

            menu.Initialise(Content);
            characterSelect.Initialise(Content);
            registerAccount.Initialise(Content);
            gameScene.Initialise(Content);
        }

        public void ClearCharacters()
        {
            if (_gameState == GameState.LoggedIn)
                characterSelect.ClearCharacters();
            else if (_gameState == GameState.Playing)
                gameScene.ClearCharacters();
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
                gameScene.AddEnemy(enemy);
            }
        }

        public void AddPlayerCharacter(PlayerCharacter character)
        {
            if (_gameState == GameState.LoggedIn)
                characterSelect.AddCharacter(character);
            else if (_gameState == GameState.Playing)
                gameScene.AddCharacter(character);
        }

        public List<PlayerCharacter> GetPlayers()
        {
            if (_gameState == GameState.LoggedIn) return characterSelect.PlayerCharacters;
            else if (_gameState == GameState.Playing) return gameScene.PlayerCharacters;
            else return null;
        }

        public List<Enemy> GetEnemies()
        {
            return gameScene.Enemies;
        }

        public void DisplayMessage(string message)
        {
            if (_gameState == GameState.MainMenu)
                menu.DisplayMessage(message);
            else if (_gameState == GameState.RegisterAccount)
                registerAccount.DisplayMessage(message);
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
                    menu.Update(gameTime);
                    break;
                case GameState.LoggedIn:
                    characterSelect.Update(gameTime);
                    break;
                case GameState.RegisterAccount:
                    registerAccount.Update(gameTime);
                    break;
                case GameState.Playing:
                    gameScene.Update(gameTime);
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
                    break;
                case GameState.LoggedIn:
                    characterSelect.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.RegisterAccount:
                    registerAccount.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.Playing:
                    gameScene.Draw(gameTime, _spriteBatch);
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
                    gameScene.ShowPing(ms);
                    break;
            }
        }

        internal void RemoveCharacter(PlayerCharacter toRemove)
        {
            if (_gameState == GameState.Playing)
                gameScene.RemoveCharacter(toRemove);
        }

        internal void ShowMessage(string message, string from)
        {
            if (_gameState == GameState.Playing)
                gameScene.ShowMessage(message, from);
        }
    }
}
