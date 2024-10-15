using Microsoft.Xna.Framework;

namespace Maze.Gamestate
{
    // A handle into the main Game object for other classes to access.
    static class GameHandle
    {
        public static Game game { get; set; }
    }
}
