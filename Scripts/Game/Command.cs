namespace MathPuzzle.Scripts.Game
{
    public struct Command
    {
        public CommandType CommandType;
        public DirectionType DirectionType;

        public Command(CommandType commandType, DirectionType directionType = DirectionType.None) {
            CommandType = commandType;
            DirectionType = directionType;
        }

        public Command(DirectionType directionType) {
            CommandType = CommandType.Move;
            DirectionType = directionType;
        }
    }
}