using Maze.MazeGeneration;

namespace Maze.Entities
{
    // The main player character in the maze.  Stores info about its state in the maze and the shortest path in addition to position.
    public class MainCharacter : Entity
    {
        // Current state of the player in relation to the shortest path.
        private PlayerState playerState;

        // Constructor.
        public MainCharacter(int x, int y) : base(x, y)
        {
            playerState = PlayerState.ON_SHORTEST_PATH;
        }

        // Move main character north.
        public void MoveNorth()
        {
            position.y -= 1;
        }

        // Move main character south.
        public void MoveSouth()
        {
            position.y += 1;
        }

        // Move main character east.
        public void MoveEast()
        {
            position.x += 1;
        }

        // Move main character west.
        public void MoveWest()
        {
            position.x -= 1;
        }

        // Gets the current state of the player.
        public PlayerState GetPlayerState()
        {
            return playerState;
        }

        // Sets the current state of the player.
        public void SetPlayerState(PlayerState playerState)
        {
            this.playerState = playerState;
        }
    }
}
