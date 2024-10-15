using Maze.Gamestate;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Maze.Input
{
    // Handles the setup and execution of user input and commands.  Also handles typing.
    public class InputManager
    {
        // Stores the relationship between input, game state, and commands to execute.
        private Dictionary<(Keys, GamestateLabel), InputMapEntry> inputMap;

        // Stores the current and previous states of the keyboard.
        private KeyboardState currentState;
        private KeyboardState previousState;

        // Collects keys that are pressed to then execute.
        private Keys[] keysBuffer;

        // Characters obtained by holding Shift key and a number key.
        private string[] shiftNumberCharacters = { ")", "!", "@", "#", "$", "%", "^", "&", "*", "(" };

        // Constructor.
        public InputManager() 
        {
            inputMap = new Dictionary<(Keys, GamestateLabel), InputMapEntry>();
        }

        // Registers a key and gamestate to a provided command.  Also specifies if the key is press-only or not.
        public void Register(Keys key, ICommand.Command command, GamestateLabel gameStateLabel, bool isPressOnly) 
        {
            inputMap.Add((key, gameStateLabel), new InputMapEntry(command, isPressOnly));
        }

        // Registers a key and gamestate to a provided InputMapEntry.  Also specifies if the key is press-only or not.
        private void Register(Keys key, GamestateLabel gameStateLabel, InputMapEntry inputMapEntry)
        {
            inputMap.Add((key, gameStateLabel), inputMapEntry);
        }

        // Unregisters a command associated with input key and gamestate.
        public void Unregister(Keys key, GamestateLabel gameStateLabel) 
        { 
            inputMap.Remove((key, gameStateLabel));
        }

        // Remaps a command to specified key and game state.
        public void Remap(GamestateLabel gameState, Keys oldKey, Keys newKey) 
        { 
            if(!inputMap.ContainsKey((newKey, gameState))) 
            {
                InputMapEntry remappedCommand = inputMap[(oldKey, gameState)];
                Unregister(oldKey, gameState);
                Register(newKey, gameState, remappedCommand);
            }
        }

        // Updates current keyboard state.
        public void UpdateCurrentState()
        {
            currentState = Keyboard.GetState();
        }

        // Updates previous keyboard state.
        public void UpdatePreviousState()
        {
            previousState = currentState;
        }

        // Executes all commands for keys matching current game state.
        public void ExecuteCommands(GamestateLabel gameState)
        {
            // Grab pressed keys.
            keysBuffer = currentState.GetPressedKeys();

            // Process commands based on stored pressed keys.
            for (int i = 0; i < keysBuffer.Length; i++)
            {
                // Grab a pressed key.
                Keys key = keysBuffer[i];

                // Check if command has entry in input map.
                if (inputMap.ContainsKey((key, gameState)))
                {
                    // Grab the inputMapEntry from input map.
                    InputMapEntry inputMapEntry = inputMap[ (key, gameState)];

                    // Check if command is press-only/
                    if (inputMapEntry.GetIsPressOnly())
                    {
                        // Key is press-only.

                        // Check if key can be pressed.
                        if (CanPressKey(key))
                        {
                            // Execute command.
                            inputMapEntry.GetCommand()();
                        }
                    }
                    else
                    {
                        // Key is not press-only.

                        // Check if key is currently down.
                        if (currentState.IsKeyDown(key))
                        {
                            // Execute command.
                            inputMapEntry.GetCommand()();
                        }
                    }
                }
            }
        }

        // Grabs characters that were typed and returns an updated version of input string.
        public string GetTypedCharacters(string workingString)
        {
            // Get pressed keys.
            keysBuffer = currentState.GetPressedKeys();

            // Check if shift and caps lock are pressed or enabled respectively.
            bool shiftDown = currentState.IsKeyDown(Keys.LeftShift) || currentState.IsKeyDown(Keys.RightShift);
            bool capitalMode = shiftDown || currentState.CapsLock;

            // Go through each key and insert pressed button into the working string.
            for (int i = 0; i < keysBuffer.Length; i++)
            {
                // Grab current key.
                Keys key = keysBuffer[i];

                // Check if key can be pressed.
                if (CanPressKey(key))
                {
                    // Check if backspace is pressed and string is non-empty.
                    if (key == Keys.Back && workingString.Length > 0)
                    {
                        // Backspace pressed. Remove last character from string.
                        workingString = workingString.Substring(0, workingString.Length - 1);
                    }
                    else
                    {
                        // Add character to string.
                        workingString += KeyToString(key, shiftDown, capitalMode);
                    }
                }
            }

            return workingString;
        }

        // Grabs characters that were typed and returns an updated version of input string.  Also includes a character limit check to ensure no excess typed characters.
        public string GetTypedCharacters(string workingString, int characterLimit)
        {
            // Get pressed keys.
            keysBuffer = currentState.GetPressedKeys();

            // Check if shift and caps lock are pressed or enabled respectively.
            bool shiftDown = currentState.IsKeyDown(Keys.LeftShift) || currentState.IsKeyDown(Keys.RightShift);
            bool capitalMode = shiftDown || currentState.CapsLock;

            // Go through each key and insert pressed button into the working string.
            for (int i = 0; i < keysBuffer.Length; i++)
            {
                // Grab current key.
                Keys key = keysBuffer[i];

                // Check if key can be pressed.
                if (CanPressKey(key))
                {
                    // Check if backspace is pressed and string is non-empty.
                    if (key == Keys.Back && workingString.Length > 0)
                    {
                        // Backspace pressed. Remove last character from string.
                        workingString = workingString.Substring(0, workingString.Length - 1);
                    }
                    else
                    {
                        // Check if the string's length does not exceed character limit.
                        if(workingString.Length < characterLimit)
                        {
                            // Add character to string.
                            workingString += KeyToString(key, shiftDown, capitalMode);
                        }
                    }
                }
            }

            return workingString;
        }

        // Converts pressed key into a string character.  Also checks for shift and capitalization.
        private string KeyToString(Keys key, bool shift, bool capitalMode)
        {
            // Check if key is in number row.
            if (key >= Keys.D0 && key <= Keys.D9)
            {
                // Check if shift is held.
                if (shift)
                {
                    // Grab shift character from number row corresponding to number pressed.
                    return shiftNumberCharacters[key - Keys.D0];
                }

                // Grab the number from number row.
                return ((int)key - (int)Keys.D0).ToString();
            }

            // Check if key is a letter
            if (key >= Keys.A && key <= Keys.Z)
            {
                // Determine if letter is captalized or not.
                if(!capitalMode)
                {
                    return key.ToString().ToLower();
                }
                else
                {
                    return key.ToString().ToUpper();
                }
            }

            // Characters and puncuation.
            switch (key)
            {
                case Keys.Space: return " ";
                case Keys.OemTilde: return shift ? "~" : "`";
                case Keys.OemSemicolon: return shift ? ":" : ";";
                case Keys.OemQuotes: return shift ? "\"" : "'";
                case Keys.OemQuestion: return shift ? "?" : "/";
                case Keys.OemPlus: return shift ? "+" : "=";
                case Keys.OemPipe: return shift ? "|" : "\\";
                case Keys.OemPeriod: return shift ? ">" : ".";
                case Keys.OemOpenBrackets: return shift ? "{" : "[";
                case Keys.OemCloseBrackets: return shift ? "}" : "]";
                case Keys.OemMinus: return shift ? "_" : "-";
                case Keys.OemComma: return shift ? "<" : ",";
                case Keys.OemClear: return shift ? "Clear" : "Clear";
            }

            // No valid key was pressed.
            return ""; 
        }

        // Checks if a key can be pressed or not.
        private bool CanPressKey(Keys key)
        {
            return currentState.IsKeyDown(key) && !previousState.IsKeyDown(key);
        }  
    }
}