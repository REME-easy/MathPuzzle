using MathPuzzle.Scripts.Extensions;

namespace MathPuzzle.Scripts.Game.Blocks
{
    public class Wall : Block
    {
        public Wall ()
        {
            BlockType = BlockType.Common;
            IsStatic = true;
        }

        public override void _Ready ()
        {
            this.InitNode ();
            InitOnReady ();
        }
    }
}