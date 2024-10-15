using Maze.Audio;
using Maze.Entities;
using Maze.GameData;
using Maze.Graphics;
using Maze.Input;
using Maze.Maze;
using Maze.MazeGeneration;
using Maze.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Maze.Gamestate
{
    // The main game state.  This is where the maze is generated and where gameplay occurs.
    public class MazeState : Gamestate
    {
        // Game timer variables.
        private double timerValue;

        // Maze generator.
        private MazeGenerator mazeGenerator;
        private MazeGeneration.Maze maze;

        // Maze size data and flags.
        private int mazeDimension;
        private MazeSizeFlag currentMazeSizeFlag;

        // Bread crumbs.
        private List<MazeCoordinate> breadCrumbPoints;
        private bool breadCrumbsToggle;

        // Hint mode (Full path or hint toggle flag)
        private HintMode hintMode;

        // Score data object.
        public ScoreData scoreData { get; set; }
        
        // UI mode (display high score or credits toggle flag)
        private UIMode uiMode;

        // Main character.
        private MainCharacter mainCharacter;

        // Constructor.
        public MazeState(GameStateManager gameStateManager, GraphicsManager graphicsData, AudioManager audioManager, InputManager inputManager, ScoreData scoreData)
            : base(GamestateLabel.GAME, gameStateManager, graphicsData, audioManager, inputManager)
        {
            this.scoreData = scoreData;
        }

        // Initialization.  Called when state added the GameStateManager.
        public override void Initialize()
        {
            RegisterInputs();

            // Maze and breadcrumb setup.
            mazeGenerator = new MazeGenerator();
            breadCrumbPoints = new List<MazeCoordinate>();
            breadCrumbsToggle = true;

            // Toggle flags setup.
            uiMode = UIMode.NONE;
            hintMode = HintMode.NONE;

            // Set timer to 0.
            timerValue = 0;

            // Set maze size flag to 15x15.
            currentMazeSizeFlag = MazeSizeFlag.MAZE_15x15;
        }

        // Process input.
        public override void ProcessInput()
        {
            // Update current state of keyboard.
            inputManager.UpdateCurrentState();

            // Execute buffered commands.
            inputManager.ExecuteCommands(gameStateLabel);

            // Update previous state of keyboard.
            inputManager.UpdatePreviousState();
        }

        // Draw.
        public override void Draw(GameTime gameTime)
        {
            // Clear frame.
            graphicsData.graphicsDevice.Clear(Color.Black);

            // Begin drawing.
            graphicsData.spriteBatch.Begin();

            // Draw the maze.
            DrawMaze(gameTime);

            // Draw bread crumbs (if enabled)
            if (breadCrumbsToggle == true)
            {
                DrawBreadCrumbs();
            }

            // Draw shortest path or hint based on HintMode value.
            if (hintMode == HintMode.SHORTEST_PATH)
            {
                // Shortest path should be drawn.
                DrawShortestPath(gameTime);
            }
            else if (hintMode == HintMode.HINT)
            {
                // Hint of shortest path should be drawn.
                DrawHint(gameTime);
            }

            // Draw main character
            DrawMainCharacter();

            // Draw exit
            DrawExit();

            // Draw Timer
            DrawTimer();

            // Draw Score
            DrawScore();

           
            DrawInstructions();
            

            // Draw UI based on UIMode value.
            if (uiMode == UIMode.HIGH_SCORE)
            {
                DrawHighScore();
            }
            else if (uiMode == UIMode.CREDITS)
            {
                DrawCredits();
            }

            graphicsData.spriteBatch.End();
        }

        // Update.
        public override void Update(GameTime gameTime)
        {
            // Update timer if still in maze.
            UpdateGameTimer(gameTime);
        }

        // Registers input into the Input Manager.
        protected override void RegisterInputs()
        {
            // Maze Generation commands
            inputManager.Register(Keys.F1, Generate5x5Maze, gameStateLabel, true);
            inputManager.Register(Keys.F2, Generate10x10Maze, gameStateLabel, true);
            inputManager.Register(Keys.F3, Generate15x15Maze, gameStateLabel, true);
            inputManager.Register(Keys.F4, Generate20x20Maze, gameStateLabel, true);

            // Toggle commands
            inputManager.Register(Keys.F5, ToggleHighScore, gameStateLabel, true);
            inputManager.Register(Keys.F6, ToggleCredits, gameStateLabel, true);

            inputManager.Register(Keys.P, ToggleShortestPath, gameStateLabel, true);
            inputManager.Register(Keys.H, ToggleHint, gameStateLabel, true);
            inputManager.Register(Keys.B, ToggleBreadcrumbs, gameStateLabel, true);

            // Movement commands

            // Up
            inputManager.Register(Keys.W, MoveNorth, gameStateLabel, true);
            inputManager.Register(Keys.I, MoveNorth, gameStateLabel, true);
            inputManager.Register(Keys.Up, MoveNorth, gameStateLabel, true);

            // Left
            inputManager.Register(Keys.A, MoveWest, gameStateLabel, true);
            inputManager.Register(Keys.J, MoveWest, gameStateLabel, true);
            inputManager.Register(Keys.Left, MoveWest, gameStateLabel, true);

            // Down
            inputManager.Register(Keys.S, MoveSouth, gameStateLabel, true);
            inputManager.Register(Keys.K, MoveSouth, gameStateLabel, true);
            inputManager.Register(Keys.Down, MoveSouth, gameStateLabel, true);

            // Right
            inputManager.Register(Keys.D, MoveEast, gameStateLabel, true);
            inputManager.Register(Keys.L, MoveEast, gameStateLabel, true);
            inputManager.Register(Keys.Right, MoveEast, gameStateLabel, true);

            // Exit command
            inputManager.Register(Keys.Escape, ExitGame, gameStateLabel, true);
        }

        // Called when the state is entered into.
        public override void OnEnterState()
        {
            uiMode = UIMode.NONE;

            // Generate a new maze.
            if (currentMazeSizeFlag == MazeSizeFlag.MAZE_5x5)
            {
                Generate5x5Maze();
            }
            else if (currentMazeSizeFlag == MazeSizeFlag.MAZE_10x10)
            {
                Generate10x10Maze();
            }
            else if (currentMazeSizeFlag == MazeSizeFlag.MAZE_15x15)
            {
                Generate15x15Maze();
            }
            else
            {
                Generate20x20Maze();
            }
        }

        // Updates the game timer while the maze is active.
        private void UpdateGameTimer(GameTime gameTime)
        {
            // Increment number of seconds that passed.
            timerValue += gameTime.ElapsedGameTime.TotalSeconds;

            // Check if a second has passed.
            if (timerValue >= 1.0)
            {
                // Increment the seconds value.  Reset the timer.
                scoreData.seconds += 1;
                timerValue = 0;
            }
        }

        // Draws the maze.  The game time is passed to aid in drawing a color transition effect that occurs on walls.
        private void DrawMaze(GameTime gameTime)
        {
            // Get current color from two color fade effect.
            Color color = graphicsData.twoColorFadeWalls.GetCurrentColor(gameTime);

            // Draw background
            Rectangle backgroundRectangle = new Rectangle(graphicsData.PADDING_X, graphicsData.PADDING_Y, graphicsData.mazeCellPixelDimension * maze.GetWidth(), graphicsData.mazeCellPixelDimension * maze.GetHeight());
            graphicsData.spriteBatch.Draw(graphicsData.background, backgroundRectangle, Color.White);

            // Draw each maze cell.
            for (int i = 0; i < maze.GetWidth(); i++)
            {
                for (int j = 0; j < maze.GetHeight(); j++)
                {
                    Rectangle currentRectangle = new Rectangle(graphicsData.PADDING_X + graphicsData.mazeCellPixelDimension * i, graphicsData.PADDING_Y + graphicsData.mazeCellPixelDimension * j, graphicsData.mazeCellPixelDimension, graphicsData.mazeCellPixelDimension);
                    graphicsData.spriteBatch.Draw(graphicsData.wallTextures[maze.GetMazeCell(i, j).GetCellTypeID()], currentRectangle, color);
                }
            }
        }

        // Draws the bread crumbs left behind by main character.
        private void DrawBreadCrumbs()
        {
            // Draw each bread crumb.
            for (int i = 0; i < breadCrumbPoints.Count; i++)
            {
                Rectangle currentRectangle = new Rectangle(graphicsData.PADDING_X + graphicsData.mazeCellPixelDimension * breadCrumbPoints[i].x, graphicsData.PADDING_Y + graphicsData.mazeCellPixelDimension * breadCrumbPoints[i].y, graphicsData.mazeCellPixelDimension, graphicsData.mazeCellPixelDimension);
                graphicsData.spriteBatch.Draw(graphicsData.breadCrumbTexture, currentRectangle, Color.White);
            }
        }

        // Draws the next step the player should take along the maze.  Game time is passed to aid in drawing a rainbow effect on hint.
        private void DrawHint(GameTime gameTime)
        {
            // Get current color from rainbow effect.
            Color color = graphicsData.rainbowEffect.GetColor(gameTime);

            //  Check if player is not on exit.  If not, draw the hint.
            if (maze.GetShortestPath().Count > 2)
            {
                Rectangle currentRectangle = new Rectangle(graphicsData.PADDING_X + graphicsData.mazeCellPixelDimension * maze.GetShortestPath()[maze.GetShortestPath().Count - 2].x, graphicsData.PADDING_Y + graphicsData.mazeCellPixelDimension * maze.GetShortestPath()[maze.GetShortestPath().Count - 2].y, graphicsData.mazeCellPixelDimension, graphicsData.mazeCellPixelDimension);
                graphicsData.spriteBatch.Draw(graphicsData.hintTexture, currentRectangle, color);
            }
        }

        // Draws the shortest path to the exit.  Game time is passed to aid in drawing of rainbow effect on path elements.
        private void DrawShortestPath(GameTime gameTime)
        {
            // Get current color from rainbow effect.
            Color color = graphicsData.rainbowEffect.GetColor(gameTime);

            // Draw each path element along shortest path.
            for (int i = 1; i < maze.GetShortestPath().Count - 1; i++)
            {
                Rectangle currentRectangle = new Rectangle(graphicsData.PADDING_X + graphicsData.mazeCellPixelDimension * maze.GetShortestPath()[i].x, graphicsData.PADDING_Y + graphicsData.mazeCellPixelDimension * maze.GetShortestPath()[i].y, graphicsData.mazeCellPixelDimension, graphicsData.mazeCellPixelDimension);
                graphicsData.spriteBatch.Draw(graphicsData.hintTexture, currentRectangle, color);
            }
        }

        // Draws the main character.
        private void DrawMainCharacter()
        {
            // Draw main character.
            Rectangle mainCharacterRectangle = new Rectangle(graphicsData.PADDING_X + graphicsData.mazeCellPixelDimension * mainCharacter.GetPosition().x, graphicsData.PADDING_Y + graphicsData.mazeCellPixelDimension * mainCharacter.GetPosition().y, graphicsData.mazeCellPixelDimension, graphicsData.mazeCellPixelDimension);
            graphicsData.spriteBatch.Draw(graphicsData.mainCharacterTexture, mainCharacterRectangle, Color.White);
        }

        // Draws the exit.
        private void DrawExit()
        {
            Rectangle exitRectangle = new Rectangle(graphicsData.PADDING_X + (graphicsData.mazeCellPixelDimension) * maze.GetExit().x, graphicsData.PADDING_Y + (graphicsData.mazeCellPixelDimension) * maze.GetExit().y, graphicsData.mazeCellPixelDimension, graphicsData.mazeCellPixelDimension);
            graphicsData.spriteBatch.Draw(graphicsData.exitTexture, exitRectangle, Color.White);
        }

        // Draws in-game timer text.
        private void DrawTimer()
        {
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "Time", new Vector2(graphicsData.TEXT_MARGIN_1, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 0), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, scoreData.seconds.ToString(), new Vector2(graphicsData.TEXT_MARGIN_1, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 1), Color.White);
        }

        // Draws in-game score text.
        private void DrawScore()
        {
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "Score", new Vector2(graphicsData.TEXT_MARGIN_1, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 3), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, scoreData.score.ToString(), new Vector2(graphicsData.TEXT_MARGIN_1, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 4), Color.White);
        }

        // Draws the credits screen.
        private void DrawCredits()
        {
            // Draw the background.
            Rectangle backgroundRectangle = new Rectangle(graphicsData.PADDING_X, graphicsData.PADDING_Y - 15, graphicsData.RENDERING_AREA_HEIGHT, 400);
            graphicsData.spriteBatch.Draw(graphicsData.rectangleColor, backgroundRectangle, Color.Blue);

            // Draw the text.
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "Credits", new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 0), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "Programming: Benjamin Ricks", new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 2), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "Graphics: Benjamin Ricks", new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 3), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "Background Image: Wikimedia Commons ", new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 4), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "Music: Joth", new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 5), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "Font: Codeman38", new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 6), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "All third-party assets used are", new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 8), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "free-to-use or public domain.", new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 9), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "See licenses in project directory.", new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 10), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "Press F6 to close.", new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 12), Color.White);
        }

        // Draws the in-game instruction text.
        private void DrawInstructions()
        {
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "F1: New 5x5 Maze", new Vector2(graphicsData.TEXT_MARGIN_2, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 0), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "F2: New 10x10 Maze", new Vector2(graphicsData.TEXT_MARGIN_2, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 1), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "F3: New 15x15 Maze", new Vector2(graphicsData.TEXT_MARGIN_2, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 2), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "F4: New 20x20 Maze", new Vector2(graphicsData.TEXT_MARGIN_2, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 3), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "F5: Show High Scores", new Vector2(graphicsData.TEXT_MARGIN_2, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 4), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "F6: Show Credits", new Vector2(graphicsData.TEXT_MARGIN_2, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 5), Color.White);

            graphicsData.spriteBatch.DrawString(graphicsData.font1, "W/I/Up Arrow: Move Up", new Vector2(graphicsData.TEXT_MARGIN_2, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 7), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "A/J/Left Arrow: Move Left", new Vector2(graphicsData.TEXT_MARGIN_2, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 8), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "S/K/Down Arrow: Move Down", new Vector2(graphicsData.TEXT_MARGIN_2, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 9), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "D/L/Right Arrow: Move Right", new Vector2(graphicsData.TEXT_MARGIN_2, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 10), Color.White);

            graphicsData.spriteBatch.DrawString(graphicsData.font1, "B: Toggle Breadcrumbs", new Vector2(graphicsData.TEXT_MARGIN_2, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 12), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "H: Toggle Hint", new Vector2(graphicsData.TEXT_MARGIN_2, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 13), Color.White);
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "P: Toggle Solution", new Vector2(graphicsData.TEXT_MARGIN_2, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 14), Color.White);

            graphicsData.spriteBatch.DrawString(graphicsData.font1, "Esc: Exit", new Vector2(graphicsData.TEXT_MARGIN_2, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 16), Color.White);
        }

        // Draws the high score screen.
        private void DrawHighScore()
        {
            // Draw the background.
            Rectangle backgroundRectangle = new Rectangle(graphicsData.PADDING_X, graphicsData.PADDING_Y - 15, graphicsData.RENDERING_AREA_HEIGHT, 400);
            graphicsData.spriteBatch.Draw(graphicsData.rectangleColor, backgroundRectangle, Color.Blue);

            // Draw the high score entries.
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "High Scores", new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * 0), Color.White);
            int i = -1;
            for (i = 0; i < scoreData.highScores.Count; i++)
            {
                graphicsData.spriteBatch.DrawString(graphicsData.font1, scoreData.highScores[i].GetName(), new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * (2 + i)), Color.White);
                graphicsData.spriteBatch.DrawString(graphicsData.font1, " : " + scoreData.highScores[i].GetScore().ToString(), new Vector2(graphicsData.TEXT_MARGIN_3 + 200, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * (2 + i)), Color.White);
            }

            // Draw remaining instructions.
            graphicsData.spriteBatch.DrawString(graphicsData.font1, "Press F5 to close.", new Vector2(graphicsData.TEXT_MARGIN_3, graphicsData.PADDING_Y + graphicsData.FONT_VERTICAL_SPACING * (i + 3)), Color.White);
        }

        // Generates a new maze using given maze size.
        private void GenerateNewMaze(int inputMazeDimension)
        {
            // Set maze dimension value.
            mazeDimension = inputMazeDimension;

            // Generate the maze.
            maze = mazeGenerator.GeneratePrimMaze(mazeDimension, mazeDimension);

            // Compute size of the MazeCell squares based on rendering area and maze size.
            graphicsData.mazeCellPixelDimension = graphicsData.RENDERING_AREA_HEIGHT / mazeDimension;

            // Set state needed for new maze.
            timerValue = 0;
            scoreData.ResetScoreData();
            breadCrumbPoints.Clear();

            // Spawn main character at upper-left corner.
            mainCharacter = new MainCharacter(0, 0);

            // Add a bread crumb for initial position of the the main character.
            breadCrumbPoints.Add(mainCharacter.GetPosition().DeepCopy());

            // Set currently occupied square by main character to visited.
            maze.GetMazeCell(mainCharacter.GetPosition()).SetTraversed(true);

            // Calculate shortest path to exit at current position of main character.
            maze.AStar_CalculateShortestPath(mainCharacter.GetPosition());
        }

        // Updates the state of the maze upon main character movemet.
        private void UpdateMazeState()
        {
            // Check if the main character is on the shortest path.
            bool inShortestPath = maze.CheckIfOnShortestPath(mainCharacter.GetPosition());

            if (inShortestPath)
            {
                // Player in shortest path.

                // Remove top element in maze's path.
                maze.PopPathElement();
            }
            else
            {
                // Player not in shortest path.
               
                // Add current position to the shortest path.
                maze.PushPathElement(mainCharacter.GetPosition());
            }

            // Check if the player's position after movement was visited previously.
            if (maze.GetMazeCell(mainCharacter.GetPosition()).GetTraversed() == false)
            {
                // The maze cell is unvisited.  Update player state and score accordingly
                breadCrumbPoints.Add(mainCharacter.GetPosition().DeepCopy());

                if (inShortestPath)
                {
                    // Player is in shortest path.

                    // Check if player was not in shortest path previously.
                    if (mainCharacter.GetPlayerState() == PlayerState.OFF_SHORTEST_PATH || mainCharacter.GetPlayerState() == PlayerState.OFF_SHORTEST_PATH_ADJACENT)
                    {
                        // Player was not in shortest path previously.  Update state to be on the shortest path.
                        mainCharacter.SetPlayerState(PlayerState.ON_SHORTEST_PATH);
                    }

                    // Update score.
                    scoreData.score += 5;
                }
                else
                {
                    // Player is off the shortest path.

                    if (mainCharacter.GetPlayerState() == PlayerState.ON_SHORTEST_PATH)
                    {
                        // The player was previously on shortest path.  Update state to be off shortest path, but adjacent to it.
                        mainCharacter.SetPlayerState(PlayerState.OFF_SHORTEST_PATH_ADJACENT);
                    }
                    else
                    {
                        // The player was previously already off the path.  Update state to be off shortest path.
                        mainCharacter.SetPlayerState(PlayerState.OFF_SHORTEST_PATH);
                    }

                    // Check if player was on the shortest path previously.
                    if (mainCharacter.GetPlayerState() == PlayerState.OFF_SHORTEST_PATH_ADJACENT)
                    {
                        // Update score.
                        scoreData.score += -2;
                    }
                    else
                    {
                        // Update score.
                        scoreData.score += -1;
                    }
                }
            }

            // Set the cell the player moved to as visited.
            maze.GetMazeCell(mainCharacter.GetPosition()).SetTraversed(true);

            // Check for victory.
            if (mainCharacter.GetPosition().Equals(maze.GetExit()))
            {
                // Victory has been attained.  Update gamestate to VICTORY_MENU mode.
                GotoVictoryState();
            }
        }

        // Exits the game.
        private void ExitGame()
        {
            GameHandle.game.Exit();
        }

        // Generates a new 20x20 maze.
        private void Generate20x20Maze()
        {
            // Update maze size flag.
            currentMazeSizeFlag = MazeSizeFlag.MAZE_20x20;

            // Generate the maze.
            GenerateNewMaze(20);
        }

        // Generates a new 15x15 maze.
        private void Generate15x15Maze()
        {
            // Update maze size flag.
            currentMazeSizeFlag = MazeSizeFlag.MAZE_15x15;

            // Generate the maze.
            GenerateNewMaze(15);
        }

        // Generates a new 10x10 maze.
        private void Generate10x10Maze()
        {
            // Update maze size flag.
            currentMazeSizeFlag = MazeSizeFlag.MAZE_10x10;

            // Generate the maze.
            GenerateNewMaze(10);
        }

        // Generates a new 5x5 maze.
        private void Generate5x5Maze()
        {
            // Update maze size flag.
            currentMazeSizeFlag = MazeSizeFlag.MAZE_5x5;

            // Generate the maze.
            GenerateNewMaze(5);
        }

        // Moves the character east and updates maze state accordingly.
        private void MoveEast()
        {
            // Check if character can even move to new position.
            if (maze.GetMazeCell(mainCharacter.GetPosition()).adjacentCells[(int)Directions.EAST])
            {
                // Update character position and maze state.
                mainCharacter.MoveEast();
                UpdateMazeState();
            }
        }

        // Moves the character south and updates maze state accordingly.
        private void MoveSouth()
        {
            // Check if character can even move to new position.
            if (maze.GetMazeCell(mainCharacter.GetPosition()).adjacentCells[(int)Directions.SOUTH])
            {
                // Update character position and maze state.
                mainCharacter.MoveSouth();
                UpdateMazeState();
            }
        }

        // Moves the character west and updates maze state accordingly.
        private void MoveWest()
        {
            // Check if character can even move to new position.
            if (maze.GetMazeCell(mainCharacter.GetPosition()).adjacentCells[(int)Directions.WEST])
            {
                // Update character position and maze state.
                mainCharacter.MoveWest();
                UpdateMazeState();
            }
        }

        // Moves the character north and updates maze state accordingly.
        private void MoveNorth()
        {
            // Check if character can even move to new position.
            if (maze.GetMazeCell(mainCharacter.GetPosition()).adjacentCells[(int)Directions.NORTH])
            {
                // Update character position and maze state.
                mainCharacter.MoveNorth();
                UpdateMazeState();
            }
        }

        // Toggles the UIMode to CREDITS or NONE depending if it was off or on respectively.
        private void ToggleCredits()
        {
            // Display Credits Toggle command.
            if (uiMode != UIMode.CREDITS)
            {
                uiMode = UIMode.CREDITS;
            }
            else
            {
                uiMode = UIMode.NONE;
            }
        }

        // Toggles the UIMode to HIGH_SCORE or NONE depending if it was off or on respectively.
        private void ToggleHighScore()
        {
            if (uiMode != UIMode.HIGH_SCORE)
            {
                uiMode = UIMode.HIGH_SCORE;
            }
            else
            {
                uiMode = UIMode.NONE;
            }
        }

        // Toggles the HintMode to HINT or NONE depending if it was off or on respectively.
        private void ToggleHint()
        {
            if (hintMode != HintMode.HINT)
            {
                hintMode = HintMode.HINT;
            }
            else
            {
                hintMode = HintMode.NONE;
            }
        }

        // Toggles whether or not to show bread crumbs.
        private void ToggleBreadcrumbs()
        {
            if (breadCrumbsToggle == true)
            {
                breadCrumbsToggle = false;
            }
            else
            {
                breadCrumbsToggle = true;
            }
        }

        // Toggles the HintMode to SHORTEST_PATH or NONE depending if it was off or on respectively.
        private void ToggleShortestPath()
        {
            if (hintMode != HintMode.SHORTEST_PATH)
            {
                hintMode = HintMode.SHORTEST_PATH;
            }
            else
            {
                hintMode = HintMode.NONE;
            }
        }

        // Transitions game state to VICTORY_MENU.  Sets up variables needed to display the high score screen for VICTORY_MENU mode.
        private void GotoVictoryState()
        {
            // Set gamestate to VICTORY_MENU.
            gameStateManager.TransitionGameState(GamestateLabel.VICTORY_MENU);
        }
    }
}
