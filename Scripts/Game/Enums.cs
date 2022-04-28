namespace MathPuzzle.Scripts.Game
{
    public enum DirectionType
    {
        None,
        Up,
        Down,
        Left,
        Right,
    }

    public enum CommandType
    {
        None,
        Move,
        Undo,
        Process,
        Restart,
    }

    public enum MoveType
    {
        Move,
        Refresh,
        RefreshExpression,
        RunExpression,
        Undo,
        Victory,
    }

    public enum HistoryType
    {
        Spawn,
        Delete,
        Move,
    }

    public enum ExpressionType
    {
        Formula,
        Assignment,
    }

    public enum BlockType
    {
        Player,
        Common,
        Function,
        Operator,
        Value,
    }
}