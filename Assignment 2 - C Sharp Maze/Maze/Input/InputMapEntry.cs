namespace Maze.Input
{
    // Represents an entry in Input Manager's inputMap dictionary. Holds a command and flags of the command's button being pressed and wheter it is a press only command.
    public class InputMapEntry
    {
        // Command. 
        private ICommand.Command command;

        // Whether or not the button is pressed.
        private bool isPressed;

        // Whether or not the button is press-only.
        private bool isPressOnly;

        // Constructor.
        public InputMapEntry(ICommand.Command command, bool isPressOnly)
        {
            this.command = command;
            this.isPressed = false;
            this.isPressOnly = isPressOnly;
        }

        // Gets the command associated to the entry.
        public ICommand.Command GetCommand()
        {
            return command;
        }

        // Gets isPressed state of the input command.
        public bool GetPressed()
        {
            return isPressed;
        }

        // Gets whether or not the command is a press-only command.
        public bool GetIsPressOnly()
        {
            return isPressOnly;
        }
    }
}
