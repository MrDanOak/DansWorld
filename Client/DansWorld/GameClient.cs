using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DansWorld.GameClient.GameExecution;
using DansWorld.GameClient.UI;
using DansWorld.GameClient.UI.Game;
using DansWorld.GameClient.UI.CustomEventArgs;
using DansWorld.GameClient.UI.Scenes;
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
        //used to send messages to server
        public static Net.Client NetClient;
        //resolution of the game client
        public static int HEIGHT = 720;
        public static int WIDTH = 1366;
        //used to build rectangles and other shapes that require some pixel data.
        public static Texture2D DEFAULT_TEXTURE;
        //fonts for the game
        public static SpriteFont DEFAULT_FONT;
        public static SpriteFont DEFAULT_FONT_BOLD;
        public static SpriteFont GW2_FONT;
        public static SpriteFont GW2_FONT_LARGE;
        //build version
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
            //initiating the network address of the game client
            NetClient = new Net.Client("127.0.0.1", 8081, this);
            _graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferHeight = HEIGHT;
            _graphics.PreferredBackBufferWidth = WIDTH;
            Window.AllowAltF4 = true;
            Window.AllowUserResizing = true;
            //enabling responsive window
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
            //loading all assets from the MonoGame pipeline tool
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

        /// <summary>
        /// Removes characters from whichever scene is currently active using state pattern
        /// </summary>
        public void ClearCharacters()
        {
            if (_gameState == GameState.LoggedIn)
                CharacterSelect.ClearCharacters();
            else if (_gameState == GameState.Playing)
                GameScence.ClearCharacters();
        }

        /// <summary>
        /// Adds player characters to the given scene
        /// </summary>
        /// <param name="playerCharacters"></param>
        public void AddPlayerCharacters(List<PlayerCharacter> playerCharacters)
        {
            foreach (PlayerCharacter player in playerCharacters) 
                AddPlayerCharacter(player);
        }

        /// <summary>
        /// Adds enemies to the game scene
        /// </summary>
        /// <param name="enemy"></param>
        public void AddEnemy(Enemy enemy)
        {
            if (_gameState == GameState.Playing)
            {
                GameScence.AddEnemy(enemy);
            }
        }

        /// <summary>
        /// Used to build character sprites from network packets
        /// behavious changes depending on the state of the game client
        /// </summary>
        /// <param name="character">charater to add</param>
        public void AddPlayerCharacter(PlayerCharacter character)
        {
            if (_gameState == GameState.LoggedIn)
                CharacterSelect.AddCharacter(character);
            else if (_gameState == GameState.Playing)
                GameScence.AddCharacter(character);
        }

        /// <summary>
        /// Returns a list of the characters that are currently loaded in the game
        /// </summary>
        /// <returns>List of player character</returns>
        public List<PlayerCharacter> GetPlayers()
        {
            if (_gameState == GameState.LoggedIn) return CharacterSelect.PlayerCharacters;
            else if (_gameState == GameState.Playing) return GameScence.PlayerCharacters;
            else return null;
        }

        /// <summary>
        /// Returns a list of the enemies that are currently loaded in the game
        /// </summary>
        /// <returns>List of enemy</returns>
        public List<Enemy> GetEnemies()
        {
            return GameScence.Enemies;
        }

        /// <summary>
        /// Displays a message to the game client, used for communicating between players
        /// Or for communicating from the server to the player for status upades or error messages
        /// </summary>
        /// <param name="message">message to display</param>
        /// <param name="from">who was it sent from</param>
        public void DisplayMessage(string message, string from = "")
        {
            if (_gameState == GameState.MainMenu)
                Menu.DisplayMessage(message);
            else if (_gameState == GameState.RegisterAccount)
                RegisterAccount.DisplayMessage(message);
            else if (_gameState == GameState.CreateCharacter)
                CharacterCreate.DisplayMessage(message);
            else if (_gameState == GameState.Playing)
                GameScence.DisplayMessage(message, from);
        }

        /// <summary>
        /// Grants focus to a given control for the given control set.
        /// Used for user input.
        /// </summary>
        /// <param name="toFocus"></param>
        /// <param name="controlSet"></param>
        public void Focus(Control toFocus, List<Control> controlSet)
        {
            foreach (Control control in controlSet)
            {
                control.HasFocus = (control == toFocus);
                if (control.HasFocus)
                    Console.WriteLine("{0} has focus", control.Name);
            }
        }

        protected override void UnloadContent()
        {
            //TODO: Cleanup
        }

        /// <summary>
        /// Main update loop of the game
        /// </summary>
        /// <param name="gameTime"></param>
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

        /// <summary>
        /// Main draw loop of the game
        /// </summary>
        /// <param name="gameTime"></param>
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

        /// <summary>
        /// Updates the game client state
        /// </summary>
        /// <param name="state"></param>
        public void SetState(GameState state)
        {
            _gameState = state;
        }

        /// <summary>
        /// updates the ping counter in game
        /// </summary>
        /// <param name="ms"></param>
        public void ShowPing(int ms)
        {
            switch (_gameState)
            {
                case GameState.Playing:
                    GameScence.ShowPing(ms);
                    break;
            }
        }

        /// <summary>
        /// removes a character from the array of collected characters.
        /// </summary>
        /// <param name="toRemove"></param>
        internal void RemoveCharacter(PlayerCharacter toRemove)
        {
            if (_gameState == GameState.Playing)
                GameScence.RemoveCharacter(toRemove);
        }
    }
}
