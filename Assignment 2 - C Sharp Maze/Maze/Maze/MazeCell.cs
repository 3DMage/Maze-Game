using Maze.Maze;
using System.Collections;
using System.Collections.Generic;

namespace Maze.MazeGeneration
{
    // Represents a single unit of the maze.  Stores info about position, adjacent cells, and whether or not it has been visited.
    public class MazeCell
    {
        // Stores the position of the cell in the maze.
        private MazeCoordinate position;

        // An ID used to indicate which texture should be used for drawing.
        private int cellTypeID;

        // Indicates whether the cell has been visited during maze solving.
        private bool traversed; 

        // Represents the existence of walls (false) or paths (true) adjacent to this cell.
        public BitArray adjacentCells;

        // Constructor.
        public MazeCell(int x, int y)
        {
            position = new MazeCoordinate(x, y);
            cellTypeID = -1;
            traversed = false; 
            adjacentCells = new BitArray(4, false);
        }

        // Converts the BitArray of adjacent cells into a numerical type ID for easier handling.
        public void CellConfigurationToTypeID()
        {
            int[] bitNumber = new int[1];
            adjacentCells.CopyTo(bitNumber, 0);
            cellTypeID = bitNumber[0];
        }

        // Set the cell as traversed or not, based on input.
        public void SetTraversed(bool traversed)
        {
            this.traversed = traversed;
        }

        // Get the cellTypeID of the cell.
        public int GetCellTypeID()
        {
            return cellTypeID;
        }

        // Checks if the cell has been traversed.
        public bool GetTraversed()
        {
            return traversed;
        }

        // Get the position of the MazeCell.
        public MazeCoordinate GetPosition()
        {
            return position;
        }

        // Gets the positions of adjacent cells that are open from current location.
        public List<MazeCoordinate> GetOpenAdjacentCoordinates()
        {
            // Store the open coordinates here, in order of North, East, South, West.
            List<MazeCoordinate> adjacentCoordinates = new List<MazeCoordinate>();

            // Checks each direction, if path exists, add its coordinate to the list.

            // North.
            if (adjacentCells[(int)Directions.NORTH])
            {
                adjacentCoordinates.Add(new MazeCoordinate(position.x, position.y - 1));
            }

            // East.
            if (adjacentCells[(int)Directions.EAST])
            {
                adjacentCoordinates.Add(new MazeCoordinate(position.x + 1, position.y));
            }

            // South.
            if (adjacentCells[(int)Directions.SOUTH])
            {
                adjacentCoordinates.Add(new MazeCoordinate(position.x, position.y + 1));
            }

            // West.
            if (adjacentCells[(int)Directions.WEST])
            {
                adjacentCoordinates.Add(new MazeCoordinate(position.x - 1, position.y));
            }

            return adjacentCoordinates;
        }
    }
}
