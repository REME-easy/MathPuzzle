using System.Collections.Generic;

using Godot;

namespace MathPuzzle.Scripts.Game
{
    public struct MathExpression
    {
        public GameMap RootMap;
        public List<Block> Blocks;
        public DirectionType OutputDirection;
        public List<OutputPosition> OutputPositions;
        public ExpressionType Type;

        public MathExpression (DirectionType outputDirection, ExpressionType type)
        {
            RootMap = null;
            Blocks = new List<Block> ();
            OutputDirection = outputDirection;
            OutputPositions = new List<OutputPosition> ();
            Type = type;
        }
    }

    public struct OutputPosition
    {
        public GameMap Map;
        public Vector2 Position;

        public OutputPosition (GameMap map, Vector2 position)
        {
            Map = map;
            Position = position;
        }
    }
}