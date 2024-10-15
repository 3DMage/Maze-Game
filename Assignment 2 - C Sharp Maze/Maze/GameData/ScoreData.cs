using System.Collections.Generic;

namespace Maze.GameData
{
    // An object that holds the current player's score and time it takes to solve a maze, as well as all the high scores.
    public class ScoreData
    {
        // Current player's score and time.
        public int score { get; set; }
        public int seconds { get; set; }

        // High scores table
        public List<HighscoreEntry> highScores { get; set; }

        // Constructor.
        public ScoreData() 
        {
            // Score state setup.
            score = 0;
            highScores = new List<HighscoreEntry>
            {
                new HighscoreEntry("Freddie M.", 250),
                new HighscoreEntry("David B.", 233),
                new HighscoreEntry("Benjamin R.", 225),
                new HighscoreEntry("Ronald McD.", 217),
                new HighscoreEntry("Wendy", 204),
                new HighscoreEntry("Sanders", 162),
                new HighscoreEntry("Mickey", 149),
                new HighscoreEntry("----", 0),
                new HighscoreEntry("----", 0),
                new HighscoreEntry("----", 0)
            };
        }

        // Resets the current player score and time to 0.
        public void ResetScoreData()
        {
            score = 0;
            seconds = 0;
        }
    }
}
