using Microsoft.Xna.Framework;

namespace Maze.Input
{
    // An interface that holds delegate sigantures for user input.
    public interface ICommand
    {
        public delegate void Command();
        public delegate void Command_Timed(GameTime gameTime);
    }
}
