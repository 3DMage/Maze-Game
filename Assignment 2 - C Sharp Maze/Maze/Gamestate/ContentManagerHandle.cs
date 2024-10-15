using Microsoft.Xna.Framework.Content;

namespace Maze.Gamestate
{
    static class ContentManagerHandle
    {
        // A handle into the Content Manager object for other classes to access.
        public static ContentManager Content { get; set; }
    }
}
