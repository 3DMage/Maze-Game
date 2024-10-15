using Maze.Audio;
using Maze.GameData;
using Maze.Gamestate;
using Maze.Graphics;
using Maze.Input;
using Microsoft.Xna.Framework;

namespace Maze
{
    // Main game class.
    public class MainGame : Game
    {
        // Managers for different aspects of the game.
        private GameStateManager gameStateManager;
        private GraphicsDeviceManager graphicsDeviceManager;
        private GraphicsManager graphicsData;
        private InputManager inputManager;
        private AudioManager audioManager;

        // Data for storing the score and time.
        private ScoreData scoreData;

        // Initialize foundational settings for game.
        public MainGame()
        {
            // Grab handles to allow state classes to access certain functionalities.
            ContentManagerHandle.Content = Content;
            GameHandle.game = this;

            // Initialize core components.
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        // Initializtion method.
        protected override void Initialize()
        {
            // Initialize managers.
            gameStateManager = new GameStateManager();
            graphicsData = new GraphicsManager(graphicsDeviceManager, GraphicsDevice);
            inputManager = new InputManager();
            audioManager = new AudioManager();

            // Initialize score data.
            scoreData = new ScoreData();

            // Configure screen settings.
            graphicsData.ConfigureGraphicsDevice();

            base.Initialize();
        }

        // Loading method.
        protected override void LoadContent()
        {
            // Load assets.
            graphicsData.LoadGraphicsAssets();
            audioManager.LoadAudio();

            // Configure audio.
            audioManager.ConfigureAudio();

            // Set up game states.
            gameStateManager.AddGameState(GamestateLabel.GAME, new MazeState(gameStateManager,graphicsData,audioManager,inputManager, scoreData));
            gameStateManager.AddGameState(GamestateLabel.VICTORY_MENU, new VictoryState(gameStateManager, graphicsData, audioManager, inputManager, scoreData));
            
            // Mark initial game state.
            gameStateManager.TransitionGameState(GamestateLabel.GAME);
        }

        // Update method.
        protected override void Update(GameTime gameTime)
        {
            // Process input and perform updates for current game state.
            gameStateManager.currentGameState.ProcessInput();
            gameStateManager.currentGameState.Update(gameTime);
            
            base.Update(gameTime);
        }

        // Draw method.
        protected override void Draw(GameTime gameTime)
        {
            // Draw current game state.
            gameStateManager.currentGameState.Draw(gameTime);
            
            base.Draw(gameTime);
        }
    }
}