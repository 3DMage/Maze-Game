using System;

namespace Maze.MazeGeneration
{
    // Represents a position in the maze.
    public class MazeCoordinate
    {
        // Maze coordinate values.
        public int x;
        public int y;

        // Default Constructor.
        public MazeCoordinate()
        {
            x = 0;
            y = 0;
        }

        // Constructor.
        public MazeCoordinate(int x, int y)
        {
            this.x = x; 
            this.y = y; 
        }

        // Creates a deep copy of the MazeCoordinate instance.
        public MazeCoordinate DeepCopy()
        {
            return new MazeCoordinate(x, y);
        }

        // Allows comparison between two MazeCoordinates.
        override public bool Equals(object other)
        {
            if (other == null || !this.GetType().Equals(other.GetType()))
            {
                // Returns false if the other object is null or not of the same type.
                return false; 
            }

            MazeCoordinate otherMazeCoordinate = (MazeCoordinate)other;

            // Compare the x and y values for equality.
            return (x == otherMazeCoordinate.x) && (y == otherMazeCoordinate.y); 
        }

        // Provides a hash code based on x and y values.
        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }
    }
}
