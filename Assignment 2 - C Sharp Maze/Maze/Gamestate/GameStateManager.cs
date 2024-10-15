using System.Collections.Generic;

namespace Maze.Gamestate
{
    // This stores all possible game states and manages what is the current game state.
    public class GameStateManager
    {
        // Holds all the game states.
        public Dictionary<GamestateLabel, Gamestate> gameStates { get; private set; }

        // The current game state in use.
        public Gamestate currentGameState { get; private set; }

        // Constructor.
        public GameStateManager()
        {
            gameStates = new Dictionary<GamestateLabel, Gamestate>();
        }

        // Adds a new game state to the game states dictonary.
        public void AddGameState(GamestateLabel gameStateLabel, Gamestate gameState)
        {
            gameStates.Add(gameStateLabel, gameState);
            gameState.Initialize();
        }

        // Goes to one game state to another.  If no current game state exists, just enter into the input game state.
        public void TransitionGameState(GamestateLabel gamestateLabel)
        {
            if(gameStates.ContainsKey(gamestateLabel) && currentGameState != null)
            {
                currentGameState.OnExitState();
                currentGameState = gameStates[gamestateLabel];
                currentGameState.OnEnterState();
            }
            else
            {
                currentGameState = gameStates[gamestateLabel];
                currentGameState.OnEnterState();
            }
        }
    }
}
