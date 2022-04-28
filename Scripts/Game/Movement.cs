using Godot;

namespace MathPuzzle.Scripts.Game
{
    public struct Movement
    {
        public int ProcessOrder;
        public Block Block;

        public GameMap FromMap;
        // public Box FromBox;
        public Vector2 From;

        public GameMap ToMap;
        // public Box ToBox;
        public Vector2 To;

        public MoveType MovementType;
        public DirectionType Direction;

        public override string ToString ()
        {
            return $"[move: {MovementType},block:{Block?.GetType().Name},from:{From},to:{To}]";
        }

        public static Movement CreateSameMapMove (Block block, GameMap map, DirectionType direction)
        {
            return new Movement
            {
                ProcessOrder = 10,
                    Block = block,
                    FromMap = map,
                    From = block.MapPosition,
                    ToMap = map,
                    To = block.MapPosition + MoveHpr.GetMoveOffset (direction),
                    MovementType = MoveType.Move,
                    Direction = direction
            };
        }

        public static Movement CreateRefreshCheck (Block block)
        {
            return new Movement
            {
                ProcessOrder = 5 + block.ProcessOrder,
                    Block = block,
                    MovementType = MoveType.Refresh
            };
        }

        public static Movement CreateRefreshExpression ()
        {
            return new Movement
            {
                ProcessOrder = 4,
                    MovementType = MoveType.RefreshExpression
            };
        }

        public static Movement CreateRunExpression ()
        {
            return new Movement
            {
                ProcessOrder = 3,
                    MovementType = MoveType.RunExpression
            };
        }

        public static Movement CreateUndo ()
        {
            return new Movement
            {
                ProcessOrder = 1,
                    MovementType = MoveType.Undo
            };
        }

        public static Movement CreateVictoryCheck ()
        {
            return new Movement
            {
                ProcessOrder = 0,
                    MovementType = MoveType.Victory
            };
        }
    }
}