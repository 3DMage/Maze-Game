using Maze.Audio;
using Maze.Graphics;
using Maze.Input;
using Microsoft.Xna.Framework;

namespace Maze.Gamestate
{
    // The base class that represents a game state.
    public abstract class Gamestate
    {
        // The label indicating which game state this is.
        public GamestateLabel gameStateLabel { get; private set; }

        // Managers for managing various aspects of the game.
        protected GraphicsManager graphicsData;
        protected InputManager inputManager;
        protected AudioManager audioManager;
        protected GameStateManager gameStateManager;

        // Constructor.
        protected Gamestate(GamestateLabel gameStateLabel, GameStateManager gameStateManager, GraphicsManager graphicsData, AudioManager audioManager, InputManager inputManager)
        {
            this.gameStateLabel = gameStateLabel;
            this.graphicsData = graphicsData;
            this.audioManager = audioManager;
            this.inputManager = inputManager;
            this.gameStateManager = gameStateManager;
        }

        // Process input.
        public abstract void ProcessInput();

        // Draw
        public abstract void Draw(GameTime gameTime);

        // Update
        public abstract void Update(GameTime gameTime);

        // Initialization.  Called when state added the GameStateManager.
        public abstract void Initialize();

        // Called when the state is entered into.  Optional.
        public virtual void OnEnterState() { }

        // Called when the state is exited from.  Optional.
        public virtual void OnExitState() { }

        // Registers input into the Input Manager.
        protected abstract void RegisterInputs();
    }
}
