namespace Maze.MazeGeneration
{
    // Represents an element to form a path in a maze.
    public class Path
    {
        // Current position on the path.
        public MazeCoordinate location;

        // Previous position on the path.
        public Path previousLocation;

        // Constructor.
        public Path(MazeCoordinate location, Path previousLocation)
        {
            this.location = location;
            this.previousLocation = previousLocation;
        }

        // Deep copy method.
        public Path DeepCopy()
        {
            return new Path(location, previousLocation);
        }
    }
}
