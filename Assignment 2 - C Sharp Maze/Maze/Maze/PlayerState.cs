namespace Maze.MazeGeneration
{
    // This is a flag that mark's the player's state in relation to their location on the shortest path.
    public enum PlayerState
    {
        ON_SHORTEST_PATH,
        OFF_SHORTEST_PATH_ADJACENT,
        OFF_SHORTEST_PATH
    }
}
