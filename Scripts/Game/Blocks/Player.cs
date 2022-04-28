using Godot;

using MathPuzzle.Scripts.Attributes;
using MathPuzzle.Scripts.Extensions;
using MathPuzzle.Scripts.Game;

namespace Scripts.Game.Blocks
{
    public class Player : Block
    {
        public Player ()
        {
            BlockType = BlockType.Common;
            IsPlayer = true;
        }

        public override void _Ready ()
        {
            this.InitNode ();
            InitOnReady ();
        }
    }
}