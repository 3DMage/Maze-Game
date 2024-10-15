using System;
using System.Collections.Generic;
using System.Linq;

namespace Maze.MazeGeneration
{
    // This class holds the data needed to represent a maze, as well as the shortest path in the maze.
    public class Maze
    {
        // Dimensions of the maze.
        private int width;
        private int height;

        // The actual maze consisting of MazeCells.
        private MazeCell[,] maze;

        // The point where the exit is.
        private MazeCoordinate exitPoint;

        // Stores the shortest path to exit.
        private List<MazeCoordinate> shortestPath;

        // Constructor to initialize the maze with specified dimensions.
        public Maze(int width, int height)
        {
            // Set maze dimensions.
            this.width = width;
            this.height = height;

            // Create a maze with specified dimensions.
            maze = new MazeCell[width, height];

            // Initialize each cell in the maze with empty MazeCell.
            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                {
                    maze[i, j] = new MazeCell(i, j);
                }
            }

            // Initialize the list to store the shortest path.
            shortestPath = new List<MazeCoordinate>();
        }

        // Returns the height of the maze.
        public int GetHeight()
        {
            return height;
        }

        // Returns the width of the maze.
        public int GetWidth()
        {
            return width;
        }

        // Returns the MazeCell at a given x, y coordinate.
        public MazeCell GetMazeCell(int x, int y)
        {
            return maze[x, y];
        }

        // Overloaded method to return the MazeCell at a given MazeCoordinate position.
        public MazeCell GetMazeCell(MazeCoordinate position)
        {
            return maze[position.x, position.y];
        }

        // Calculates the shortest path from a start position to the exit point using the A* algorithm.
        public void AStar_CalculateShortestPath(MazeCoordinate startPosition)
        {
            // Initializes a priority queue to manage nodes by their fScore values for efficient retrieval.
            PriorityQueue<MazeCoordinate, int> exportationQueue = new PriorityQueue<MazeCoordinate, int>();

            // Maps each node to its best predecessor found during the search.
            Dictionary<MazeCoordinate, MazeCoordinate> cameFrom = new Dictionary<MazeCoordinate, MazeCoordinate>();

            // Stores the cost of the cheapest path from start to each node.
            Dictionary<MazeCoordinate, int> gScores = new Dictionary<MazeCoordinate, int> { [startPosition] = 0 };

            // Stores the estimated total cost from start to the exit through each node.
            Dictionary<MazeCoordinate, int> fScores = new Dictionary<MazeCoordinate, int> { [startPosition] = CalculateManhattanDistance(startPosition, exitPoint) };

            // Placeholder for the current node being processed.
            MazeCoordinate currentLocation = new MazeCoordinate();

            // Adds the start position to the open set with its fScore as priority.
            exportationQueue.Enqueue(startPosition, fScores[startPosition]);

            // Continues searching while there are nodes left to explore.
            while (exportationQueue.Count > 0)
            {
                // Dequeues the node with the lowest fScore from the open set.
                currentLocation = exportationQueue.Dequeue();

                // Checks if the current node is the exit. If so, the search is complete.
                if (currentLocation.Equals(exitPoint))
                {
                    break;
                }

                // Retrieves all adjacent coordinates of the current MazeCell location.
                List<MazeCoordinate> neighboringCells = maze[currentLocation.x, currentLocation.y].GetOpenAdjacentCoordinates();

                // Iterates through each neighbor of the current node.
                for (int i = 0; i < neighboringCells.Count; i++)
                {
                    // Current gScore obtained from current neighboring location.
                    int currentGScore;

                    // GScore computed when moving to the neighbor.
                    int tentativeGScore;

                    // Check if a gScore is found at the current location.
                    if (gScores.TryGetValue(currentLocation, out currentGScore))
                    {
                        // Update tentativeGScore.  Assume a cost of 1 to move to a neighbor.
                        tentativeGScore = currentGScore + 1; 
                    }
                    else
                    {
                        // No value found in gScores.  Assign a high value to skip the next check.
                        tentativeGScore = int.MaxValue; 
                    }

                    // Update tracked neighboring coordinate in maze.
                    MazeCoordinate currentNeighborPoint = neighboringCells[i];

                    // Checks if the path through the current node to this neighbor is better than any previously found.
                    if (tentativeGScore < gScores.GetValueOrDefault(currentNeighborPoint, int.MaxValue))
                    {
                        // Updates the path to this neighbor with the new, better path.
                        cameFrom[currentNeighborPoint] = currentLocation;
                        gScores[currentNeighborPoint] = tentativeGScore;
                        fScores[currentNeighborPoint] = tentativeGScore + CalculateManhattanDistance(currentNeighborPoint, exitPoint);

                        // Adds the neighbor to the open set if it's not already present.
                        if (!exportationQueue.UnorderedItems.Any(item => item.Element.Equals(currentNeighborPoint)))
                        {
                            exportationQueue.Enqueue(currentNeighborPoint, fScores[currentNeighborPoint]);
                        }
                    }
                }
            }

            // Once the exit is reached or no nodes are left to explore, reconstruct the path from the exit to the start.
            ReconstructPath(cameFrom, currentLocation);
        }

        // Sets the exit point of the maze.
        public void SetExit(MazeCoordinate exitPoint)
        {
            this.exitPoint = exitPoint;
        }

        // Gets the current exit point of the maze.
        public MazeCoordinate GetExit()
        {
            return exitPoint;
        }

        // Returns the shortest path calculated from the start position to the exit.
        public List<MazeCoordinate> GetShortestPath()
        {
            return shortestPath;
        }

        // Removes the last element from the shortest path.
        public void PopPathElement()
        {
            // Check to see if any "path" is left.
            if (shortestPath.Count > 0)
            {
                // Remove last element from List.
                shortestPath.RemoveAt(shortestPath.Count - 1);
            }
        }

        // Adds a new element to the end of the shortest path.
        public void PushPathElement(MazeCoordinate pathLocation)
        {
            shortestPath.Add(pathLocation.DeepCopy());
        }

        // Checks if a given position is on the current shortest path.
        public bool CheckIfOnShortestPath(MazeCoordinate position)
        {
            return shortestPath.Contains(position);
        }

        // Calculates the Manhattan distance between two maze coordinates.
        private int CalculateManhattanDistance(MazeCoordinate a, MazeCoordinate b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        // Reconstructs the shortest path from the exit point back to the start position.
        private void ReconstructPath(Dictionary<MazeCoordinate, MazeCoordinate> cameFrom, MazeCoordinate currentLocation)
        {
            // Add current location to shortest path.
            shortestPath.Add(currentLocation.DeepCopy());

            // Backtrack through cameFrom map and update shortestPath.
            while (cameFrom.ContainsKey(currentLocation))
            {
                currentLocation = cameFrom[currentLocation];
                shortestPath.Add(currentLocation.DeepCopy()); 
            }
        }
    }
}