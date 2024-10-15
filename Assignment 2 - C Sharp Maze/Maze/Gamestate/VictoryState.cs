using Maze.Audio;
using Maze.GameData;
using Maze.Graphics;
using Maze.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Maze.Gamestate
{
    // The victory state.  Displays final score, as well as the high scores.  If player achieves new high score, they can enter their name on the leaderboard.
    public class VictoryState : Gamestate
    {
        // Player and score data
        private string playerName;
        private int scorePlacement;

        // Utilities for displaying cursor for text input.
        private double cursorBlinkTimer;
        private bool drawCursorFlag;
        private bool highScoreAchieved;
        private const int HIGH_SCORE_NAME_CHARACTER_LIMIT = 12;
        private const double CURSOR_BLINK_INTERVAL = 500;

        // Score data object.
        private ScoreData scoreData;

        // Constructor.
        public VictoryState(GameStateManager gameStateManager, GraphicsManager graphicsData, AudioManager audioManager, InputManager inputManager, ScoreData scoreData) 
            : base(GamestateLabel.VICTORY_MENU, gameStateManager, graphicsData, audioManager, inputManager)
        {
            this.scoreData = scoreData;
        }

        // Initialization.  Called when state added the GameStateManager.
        public override void Initialize()
        {
            RegisterInputs();

            playerName = "";
            cursorBlinkTimer = 0.0;
            drawCursorFlag = false;
            highScoreAchieved = false;
        }

        // Process input.
        public override void ProcessInput()
        {
            // Update current state of keyboard.
            inputManager.UpdateCurrentState();

            // Check if the player should type in their name.
            if(highScoreAchieved)
            {
                // Obtain the pressed charaters for player name.
                playerName = inputManager.GetTypedCharacters(playerName, HIGH_SCORE_NAME_CHARACTER_LIMIT);

                // Update the highscore entry as necessary.
                scoreData.highScores[scorePlacement].SetName(playerName);
            }
           
            // Execute buffered commands.
            inputManager.ExecuteCommands(gameStateLabel);

            // Update previous state of keyboard.
            inputManager.UpdatePreviousState();
        }

        // Draw.
        public override void Draw(GameTime gameTime)
        {
            DrawVictoryScreen();
        }

        // Update.
        public override void Update(GameTime gameTime)
        {
            // Update background timer.  This is used to notify when to draw or not draw the blinking cursor when inputting a name for a high score.
            UpdateCursorTimer(gameTime);
        }

        // Registers input into the Input Manager.
        protected override void RegisterInputs()
        {
            // Victory Screen commands
            inputManager.Register(Keys.F5, GotoGameState, gameStateLabel, true);
            inputManager.Register(Keys.Escape, ExitGame, gameStateLabel, true);
        }

        // Called when the state is entered into.
        public override void OnEnterState()
        {
            // Check if score beats any of the high scores.  If so, update scorePlacement variable.
            int index = 0;
            while (index < scoreData.highScores.Count)
            {
                // Check if score beats the current index in high score table.
                if (scoreData.score >= scoreData.highScores[index].GetScore())
                {
                    // Your score exceeds or ties with the current score entry.

                    // Insert your score in place of the old score.
                    scoreData.highScores.Insert(index, new HighscoreEntry("", scoreData.score));
                    scoreData.highScores.RemoveAt(scoreData.highScores.Count - 1);

                    // Obtain info needed for display of high score board and typing player name into the scoreboard.
                    scorePlacement = index;
                    highScoreAchieved = true;

                    // Exit the loop.
                    break;
                }

                // Increment the index.
                index++;
            }
        }

        // Called when the state is exited from.
        public override void OnExitState()
        {
            // Reset necessary variables for next round.
            scoreData.ResetScoreData();
            playerName = "";
            highScoreAchieved = false;
            scorePlacement = -1;
        }

        // Updates the background timer while in VICTORY_MENU mode.
        private void UpdateCursorTimer(GameTime gameTime)
        {
            // Obtain time that passed since last update.
            cursorBlinkTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

            // Draw or don't draw the blinking cursor depending on the drawCursor flag.
            if (cursorBlinkTimer >= CURSOR_BLINK_INTERVAL)
            {
                if (drawCursorFlag == true)
                {
                    // Don't show the cursor.
                    drawCursorFlag = false;
                }
                else
                {
                    // Shot the cursor.
                    drawCursorFlag = true;
                }

                // Reset the timer.
                cursorBlinkTimer = 0.0;
            }
        }

        // Draws the victory screen.
        private void DrawVictoryScreen()
        {
            // Clear frame.
            graphicsData.graphicsDevice.Clear(Color.Black);

            // Begin drawing.
            graphicsData.spriteBatch.Begin();

            // Draw the background.
            Rectangle backgroundRectangle = new Rectangle(graphicsData.PADDING_X, graphicsData.PADDING_Y - 15, 600, 500);
            graphicsData.spriteBatch.Draw(graphicsData.rectangleColor, backgroundRectangle, Color.Blue);

            // Draw final score and time.
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "Your Score: " + scoreData.score, new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 0), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "Time: " + scoreData.seconds + " sec", new Vector2(graphicsData.TEXT_MARGIN_3 + 350, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 0), Color.White);

            // If new high score achieved, draw a message indicating so.
            if (highScoreAchieved)
            {
                graphicsData.spriteBatch.DrawString(graphicsData.font1, "New High Score!  Enter in your name,", new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 2), Color.White);
                graphicsData.spriteBatch.DrawString(graphicsData.font1, "then press F5 to close.", new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 3), Color.White);
            }

            // Draw the high score entries.
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "High Scores", new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 5), Color.White);
            int i;
            for (i = 0; i < scoreData.highScores.Count; i++)
            {
                graphicsData.spriteBatch.DrawString(graphicsData.font1, scoreData.highScores[i].GetName(), new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * (7 + i)), Color.White);
                graphicsData.spriteBatch.DrawString(graphicsData.font1, " : " + scoreData.highScores[i].GetScore().ToString(), new Vector2(graphicsData.TEXT_MARGIN_3 + 200, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * (7 + i)), Color.White);
            }

            // Draw text input cursor for when player inputs their name in score board.
            if (drawCursorFlag == true && highScoreAchieved)
            {
                graphicsData.spriteBatch.DrawString(graphicsData.font1, "_", new Vector2(graphicsData.TEXT_MARGIN_3 + graphicsData.font1.MeasureString(scoreData.highScores[scorePlacement].GetName()).X, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * (7 + scorePlacement)), Color.White);
            }

            // Draw remaining instructions.
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "Press F5 to close.", new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * (i + 8)), Color.White);

            graphicsData.spriteBatch.End();
        }

        // Exits the game.
        private void ExitGame()
        {
            GameHandle.game.Exit();
        }

        // Transitions game state to GAME.
        private void GotoGameState()
        {
            // Set gamestate to GAME.
            gameStateManager.TransitionGameState(GamestateLabel.GAME);
        }
    }
}
