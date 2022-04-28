using MathPuzzle.Scripts.Extensions;
using MathPuzzle.Scripts.Game.Blocks.Items;

namespace MathPuzzle.Scripts.Game.Blocks.Operators
{
    public class Times : Block
    {
        public Times ()
        {
            BlockType = BlockType.Operator;
            ProcessOrder = 2;
        }

        public override void _Ready ()
        {
            this.InitNode ();
            InitOnReady ();
        }

        public override bool Operate (Block a, Block b, out Block output)
        {
            output = a;
            if (a is INumber na && b is INumber nb)
            {
                if (!IsValidVariable (a, b))
                    return false;
                var res = na.NumberInfo * nb.NumberInfo;
                if (na.NumberInfo == int.MaxValue || nb.NumberInfo == int.MaxValue) res = int.MaxValue;

                output = Number.NumberFac.Instance<Number> ();
                output.Init (new BlockParams ().AddParams ("number", (long) res));
                return true;
            }
            return false;
        }
    }
}