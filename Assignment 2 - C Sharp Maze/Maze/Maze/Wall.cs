using Maze.Maze;

namespace Maze.MazeGeneration
{
    // Represents a wall in a maze.  Used in conjunction with MazeGenerator.
    public class Wall
    {
        // The cells that share the wall.
        public MazeCoordinate cell1Coordinate;
        public MazeCoordinate cell2Coordinate;

        // The direction from Cell1 to Cell2.
        public Directions direction_Cell1ToCell2;

        // The direction from Cell2 to Cell1.
        public Directions direction_Cell2ToCell1;

        // Constructor.
        public Wall(MazeCoordinate cell1Coordinate, MazeCoordinate cell2Coordinate, Directions direction_Cell1ToCell2) 
        { 
            this.cell1Coordinate = cell1Coordinate;
            this.cell2Coordinate = cell2Coordinate;
            this.direction_Cell1ToCell2 = direction_Cell1ToCell2;

            // Assign the directions for Cell2 to Cell1 based on direction_Cell2ToCell1.
            if (direction_Cell1ToCell2 == Directions.NORTH)
            {
                direction_Cell2ToCell1 = Directions.SOUTH;
            }
            else if (direction_Cell1ToCell2 == Directions.SOUTH)
            {
                direction_Cell2ToCell1 = Directions.NORTH;
            }
            else if (direction_Cell1ToCell2 == Directions.EAST)
            {
                direction_Cell2ToCell1 = Directions.WEST;
            }
            else
            {
                direction_Cell2ToCell1 = Directions.EAST;
            }
        }
    }
}
