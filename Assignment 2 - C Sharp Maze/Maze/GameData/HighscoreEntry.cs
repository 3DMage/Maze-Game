namespace Maze.GameData
{
    // Represents an entry in the high score table.
    public class HighscoreEntry
    {
        // Name of player who achieved high score.
        private string name;

        // The score the player obtained.
        private int score;

        // Contructor.
        public HighscoreEntry(string name, int score)
        {
            this.name = name;
            this.score = score;
        }

        // Sets player name into input name.
        public void SetName(string name)
        {
            this.name = name;
        }

        // Gets the player name.
        public string GetName()
        {
            return name;
        }

        // Gets the player score.
        public int GetScore()
        {
            return score;
        }
    }
}
