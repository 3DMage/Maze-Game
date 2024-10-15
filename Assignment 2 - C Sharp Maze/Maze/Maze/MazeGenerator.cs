using Maze.Maze;
using System;
using System.Collections.Generic;

namespace Maze.MazeGeneration
{
    // A class used for generating mazes.  It utilizes Prim's algorithm to generate the maze.
    public class MazeGenerator
    {
        // Creates a new maze of the given size using Prim's algorithm.
        public Maze GeneratePrimMaze(int width, int height)
        {
            // Define maze bounds.
            MazeCoordinate minBoundary = new MazeCoordinate(0,0);
            MazeCoordinate maxBoundary = new MazeCoordinate(width,height);

            // Generate a new maze of the given size.
            Maze maze = new Maze(width, height);

            // Initialize a Random instance for random number generation.
            Random random = new Random();

            // List of visited cells.
            bool[,] visitedCells = new bool[width,height];

            // Initialize visited cells array with unvisited state (false).
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    visitedCells[i,j] = false;
                }
            }

            // Store walls to process here.
            List<Wall> wallList = new List<Wall>();

            // Compute random start point to generate maze from.
            int startX = random.Next(width);
            int startY = random.Next(height);

            #region Prim Maze Generation
            // Mark starting cell as "visited"
            visitedCells[startX, startY] = true;

            // For starting point, add walls into the wall list.  Do not add if the wall is on the maze's boundary.

            // North.
            if(startY - 1 >= minBoundary.y)
            {
                wallList.Add(new Wall(new MazeCoordinate(startX, startY), new MazeCoordinate(startX, startY - 1), Directions.NORTH));
            }

            // East.
            if (startX + 1 < maxBoundary.x)
            {
                wallList.Add(new Wall(new MazeCoordinate(startX, startY), new MazeCoordinate(startX + 1, startY), Directions.EAST));
            }

            // South.
            if (startY + 1 < maxBoundary.y)
            {
                wallList.Add(new Wall(new MazeCoordinate(startX, startY), new MazeCoordinate(startX, startY + 1), Directions.SOUTH));
            }

            // West.
            if (startX - 1 >= minBoundary.x)
            {
                wallList.Add(new Wall(new MazeCoordinate(startX, startY), new MazeCoordinate(startX -1, startY), Directions.WEST));
            }

            // Begin processing walls and building the maze.
            while (wallList.Count > 0)
            {
                // Obtain a random wall from the list.
                int index = random.Next(wallList.Count);
                Wall currentWall = wallList[index];

                // Obtain cells that share the wall.
                int cell1X = currentWall.cell1Coordinate.x;
                int cell1Y = currentWall.cell1Coordinate.y;

                int cell2X = currentWall.cell2Coordinate.x;
                int cell2Y = currentWall.cell2Coordinate.y;

                // Open the passage for corresponding cells if the neighboring cell is unvisited.
                if (visitedCells[cell2X, cell2Y] == false && visitedCells[cell1X, cell1Y] == true)
                {
                    // Open the walls.
                    maze.GetMazeCell(cell1X, cell1Y).adjacentCells[(int)currentWall.direction_Cell1ToCell2] = true;
                    maze.GetMazeCell(cell2X, cell2Y).adjacentCells[(int)currentWall.direction_Cell2ToCell1] = true;

                    // Add the cell the wall opened into to the visited cells lsit.
                    visitedCells[cell2X, cell2Y] = true;

                    // Add the walls of the newly opened cell into the wall list.  Do not add the wall if it is on the maze's boundary.

                    // North.
                    if (cell2Y - 1 >= minBoundary.y && visitedCells[cell2X, cell2Y - 1] == false)
                    {
                        wallList.Add(new Wall(new MazeCoordinate(cell2X, cell2Y), new MazeCoordinate(cell2X, cell2Y - 1), Directions.NORTH));
                    }

                    // East.
                    if (cell2X + 1 < maxBoundary.x && visitedCells[cell2X + 1, cell2Y] == false)
                    {
                        wallList.Add(new Wall(new MazeCoordinate(cell2X, cell2Y), new MazeCoordinate(cell2X + 1, cell2Y), Directions.EAST));
                    }

                    // South.
                    if (cell2Y + 1 < maxBoundary.y && visitedCells[cell2X, cell2Y + 1] == false)
                    {
                        wallList.Add(new Wall(new MazeCoordinate(cell2X, cell2Y), new MazeCoordinate(cell2X, cell2Y + 1), Directions.SOUTH));
                    }

                    // West.
                    if (cell2X - 1 >= minBoundary.x && visitedCells[cell2X - 1, cell2Y] == false)
                    {
                        wallList.Add(new Wall(new MazeCoordinate(cell2X, cell2Y), new MazeCoordinate(cell2X - 1, cell2Y), Directions.WEST));
                    }
                }
                
                // Remove the opened wall from wall list.
                wallList.RemoveAt(index);
            }
            #endregion

            #region Maze Cell Type Labeling
            // Label each cell's cellTypeID based on the BitArray.
            for (int i = 0; i < maze.GetWidth(); i++)
            {
                for (int j = 0; j < maze.GetHeight(); j++)
                {
                    MazeCell currentMazeCell = maze.GetMazeCell(i, j);
                    currentMazeCell.CellConfigurationToTypeID();
                }
            }
            #endregion
            
            // Set the maze's exit to the bottom right corner.
            maze.SetExit(new MazeCoordinate(width - 1, height - 1));

            return maze;
        }
    }
}
