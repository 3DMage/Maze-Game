using Maze.MazeGeneration;

namespace Maze.Entities
{
    // Represents an object that can be positioned on the maze.
    public class Entity
    {
        // Coordinate in maze.
        protected MazeCoordinate position;

        // Constructor.
        protected Entity(int x, int y)
        {
            position = new MazeCoordinate(x, y);
        }   

        // Gets the current position of the entity.
        public MazeCoordinate GetPosition()
        {
            return position;
        }
    }
}
